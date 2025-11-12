using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoXico.Data;
using ProjetoXico.Models;
using ProjetoXico.ViewModel;

namespace ProjetoXico.Controllers
{
    [Authorize]
    public class TbMaterialController : Controller
    {
        private readonly BancoContext _context;

        public TbMaterialController(BancoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new MaterialViewModel
            {
                Ativos = await _context.Materiais
                    .Where(m => m.FlAtivo)
                    .ToListAsync(),

                Inativos = await _context.Materiais
                    .Where(m => !m.FlAtivo)
                    .ToListAsync()
            };

            return View("~/Views/Home/Material.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                var material = await _context.Materiais.FindAsync(id);
                if (material != null)
                {
                    material.FlAtivo = false;
                    await _context.SaveChangesAsync();
                    TempData["Mensagem"] = "Material excluído com sucesso! Agora está inativo.";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao excluir material: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(TbMaterial material)
        {
            try
            {
                var existente = await _context.Materiais.FindAsync(material.IdMaterial);
                if (existente == null)
                {
                    TempData["Mensagem"] = "Material não encontrado!";
                    return RedirectToAction("Index");
                }

                existente.Descricao = material.Descricao;
                existente.UnidadeMedida = material.UnidadeMedida;
                existente.Quantidade = material.Quantidade;
                existente.Metrica = material.Metrica;
                existente.ValorUnitario = material.ValorUnitario;
                existente.FlAtivo = material.FlAtivo;

                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Material atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao atualizar material: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Inserir(TbMaterial material)
        {
            try
            {
                material.DataInsercao = DateTime.Now;
                _context.Materiais.Add(material);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Material adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao adicionar material: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
