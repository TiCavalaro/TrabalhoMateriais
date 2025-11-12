using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoXico.Data;
using ProjetoXico.Models;
using ProjetoXico.ViewModel;
using System.Security.Claims;

namespace ProjetoXico.Controllers
{
    [Authorize]
    public class TbProjetoMaterialController : Controller
    {
        private readonly BancoContext _context;

        public TbProjetoMaterialController(BancoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<IActionResult> Index(int projetoId)
        {
            try
            {
                var projeto = await _context.Projetos
                    .FirstOrDefaultAsync(p => p.IdProjeto == projetoId);

                if (projeto == null)
                {
                    TempData["Mensagem"] = "Projeto não encontrado.";
                    return RedirectToAction("Index", "TbProjeto");
                }

                var materiaisVinculados = await _context.ProjetoMateriais
                    .Include(pm => pm.Material)
                    .Where(pm => pm.IdProjeto == projetoId && !pm.FlExcluido)
                    .OrderBy(pm => pm.Material.Descricao)
                    .ToListAsync();

                var materiaisDisponiveis = await _context.Materiais
                    .Where(m => m.FlAtivo)
                    .OrderBy(m => m.Descricao)
                    .ToListAsync();

                var viewModel = new ProjetoMaterialViewModel
                {
                    Projeto = projeto,
                    MateriaisVinculados = materiaisVinculados,
                    MateriaisDisponiveis = materiaisDisponiveis
                };

                return View("~/Views/Home/ProjetoMaterial.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao carregar materiais do projeto: {ex.Message}";
                return RedirectToAction("Index", "TbProjeto");
            }
        }

        [HttpGet]
        public IActionResult Materiais(int id)
        {
            return RedirectToAction("Index", new { projetoId = id });
        }

        [HttpPost]
        public async Task<IActionResult> Inserir(TbProjetoMaterial projetoMaterial)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["Mensagem"] = "Erro: usuário não autenticado.";
                    return RedirectToAction("Login", "Auth");
                }

                var material = await _context.Materiais
                    .FirstOrDefaultAsync(m => m.IdMaterial == projetoMaterial.IdMaterial);

                if (material == null)
                {
                    TempData["Mensagem"] = "❌ Material não encontrado!";
                    return RedirectToAction("Index", new { projetoId = projetoMaterial.IdProjeto });
                }

                if (projetoMaterial.QuantidadeUtilizada <= 0)
                {
                    TempData["Mensagem"] = "❌ Quantidade inválida! Insira um valor positivo.";
                    return RedirectToAction("Index", new { projetoId = projetoMaterial.IdProjeto });
                }

                if (projetoMaterial.QuantidadeUtilizada > material.Quantidade)
                {
                    TempData["Mensagem"] = $"❌ Quantidade insuficiente! Estoque atual: {material.Quantidade}.";
                    return RedirectToAction("Index", new { projetoId = projetoMaterial.IdProjeto });
                }

                material.Quantidade -= projetoMaterial.QuantidadeUtilizada;

                projetoMaterial.DataInsercao = DateTime.Now;
                projetoMaterial.IdColaboradorInsercao = int.Parse(userId);
                projetoMaterial.FlExcluido = false;

                _context.ProjetoMateriais.Add(projetoMaterial);
                await _context.SaveChangesAsync();

                TempData["Mensagem"] = "✅ Material adicionado ao projeto e estoque atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao adicionar material: {ex.Message}";
            }

            return RedirectToAction("Index", new { projetoId = projetoMaterial.IdProjeto });
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(TbProjetoMaterial projetoMaterial)
        {
            try
            {
                var existente = await _context.ProjetoMateriais
                    .FirstOrDefaultAsync(pm => pm.IdProjetoMaterial == projetoMaterial.IdProjetoMaterial);

                if (existente == null)
                {
                    TempData["Mensagem"] = "❌ Material não encontrado!";
                    return RedirectToAction("Index", new { projetoId = projetoMaterial.IdProjeto });
                }

                var materialAntigo = await _context.Materiais
                    .FirstOrDefaultAsync(m => m.IdMaterial == existente.IdMaterial);

                var materialNovo = await _context.Materiais
                    .FirstOrDefaultAsync(m => m.IdMaterial == projetoMaterial.IdMaterial);

                if (projetoMaterial.QuantidadeUtilizada <= 0)
                {
                    TempData["Mensagem"] = "❌ Quantidade inválida! Insira um valor positivo.";
                    return RedirectToAction("Index", new { projetoId = projetoMaterial.IdProjeto });
                }

                if (materialAntigo != null)
                    materialAntigo.Quantidade += existente.QuantidadeUtilizada;

                if (materialNovo == null)
                {
                    TempData["Mensagem"] = "❌ Material não encontrado!";
                    return RedirectToAction("Index", new { projetoId = projetoMaterial.IdProjeto });
                }

                if (projetoMaterial.QuantidadeUtilizada > materialNovo.Quantidade)
                {
                    TempData["Mensagem"] = $"❌ Quantidade insuficiente! Estoque atual: {materialNovo.Quantidade}.";
                    return RedirectToAction("Index", new { projetoId = projetoMaterial.IdProjeto });
                }

                materialNovo.Quantidade -= projetoMaterial.QuantidadeUtilizada;

                existente.QuantidadeUtilizada = projetoMaterial.QuantidadeUtilizada;
                existente.FlAdicional = projetoMaterial.FlAdicional;
                existente.IdMaterial = projetoMaterial.IdMaterial;

                await _context.SaveChangesAsync();

                TempData["Mensagem"] = "✏️ Material atualizado e estoque ajustado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao atualizar material: {ex.Message}";
            }

            return RedirectToAction("Index", new { projetoId = projetoMaterial.IdProjeto });
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                var projetoMaterial = await _context.ProjetoMateriais.FindAsync(id);
                if (projetoMaterial == null)
                {
                    TempData["Mensagem"] = "❌ Material não encontrado!";
                    return RedirectToAction("Index", "TbProjeto");
                }

                var material = await _context.Materiais.FindAsync(projetoMaterial.IdMaterial);
                if (material != null)
                    material.Quantidade += projetoMaterial.QuantidadeUtilizada;

                projetoMaterial.FlExcluido = true;

                await _context.SaveChangesAsync();

                TempData["Mensagem"] = "🗑️ Material removido do projeto e estoque atualizado com sucesso!";
                return RedirectToAction("Index", new { projetoId = projetoMaterial.IdProjeto });
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao excluir material: {ex.Message}";
                return RedirectToAction("Index", new { projetoId = id });
            }
        }
    }
}
