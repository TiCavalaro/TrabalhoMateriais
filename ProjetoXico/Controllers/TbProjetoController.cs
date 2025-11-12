using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoXico.Data;
using ProjetoXico.Models;
using ProjetoXico.ViewModel;
using System.Security.Claims;

[Authorize]
public class TbProjetoController : Controller
{
    private readonly BancoContext _context;

    public TbProjetoController(BancoContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var viewModel = new ProjetoViewModel
            {
                Ativos = await _context.Projetos
                    .Where(p => p.FlAtivo)
                    .OrderBy(p => p.NomeProjeto)
                    .ToListAsync(),

                Inativos = await _context.Projetos
                    .Where(p => !p.FlAtivo)
                    .OrderBy(p => p.NomeProjeto)
                    .ToListAsync(),

                TiposProjeto = await _context.TipoProjetos
                    .Where(t => t.FlAtivo)
                    .OrderBy(t => t.Descricao)
                    .ToListAsync()
            };

            return View("~/Views/Home/Projeto.cshtml", viewModel);
        }
        catch (Exception ex)
        {
            TempData["Mensagem"] = $"Erro ao carregar projetos: {ex.Message}";
            return View("~/Views/Home/Projeto.cshtml", new ProjetoViewModel());
        }
    }

    [HttpPost]
    public async Task<IActionResult> Inserir(TbProjeto projeto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Mensagem"] = "Erro: usuário não autenticado.";
                return RedirectToAction("Login", "Auth");
            }

            await _context.Database.ExecuteSqlRawAsync(
                "CALL InserirProjeto({0}, {1}, {2}, {3}, {4})",
                projeto.NomeProjeto,
                projeto.DataPlanejadaInicio,
                projeto.DataPlanejadaFim,
                projeto.TipoProjeto,
                int.Parse(userId)
            );

            TempData["Mensagem"] = "Projeto adicionado com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Mensagem"] = $"Erro ao adicionar projeto: {ex.Message}";
        }

        return RedirectToAction("Index");
    }

    // 🔸 Atualizar via procedure
    [HttpPost]
    public async Task<IActionResult> Atualizar(TbProjeto projeto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Mensagem"] = "Erro: usuário não autenticado.";
                return RedirectToAction("Login", "Auth");
            }

            await _context.Database.ExecuteSqlRawAsync(
                "CALL AtualizarProjeto(@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7)",
                parameters: new object[]
                {
                projeto.IdProjeto,
                projeto.NomeProjeto,
                projeto.DataPlanejadaInicio,
                projeto.DataPlanejadaFim,
                projeto.TipoProjeto,
                projeto.FlAtivo,
                projeto.FlFinalizado,
                int.Parse(userId) // <-- adicionamos aqui!
                });

            TempData["Mensagem"] = "Projeto atualizado com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Mensagem"] = $"Erro ao atualizar projeto: {ex.Message}";
        }

        return RedirectToAction("Index");
    }


    [HttpPost]
    public async Task<IActionResult> Excluir(int id)
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync(
                "CALL ExcluirProjeto({0})",
                id
            );

            TempData["Mensagem"] = "Projeto excluído com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Mensagem"] = $"Erro ao excluir projeto: {ex.Message}";
        }

        return RedirectToAction("Index");
    }
}
