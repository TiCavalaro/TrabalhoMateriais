using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoXico.Data;
using ProjetoXico.Models;

namespace ProjetoXico.Controllers
{
    [Authorize]
    public class TbTipoProjetoController : Controller
    {
        private readonly BancoContext _context;


        public TbTipoProjetoController(BancoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new TipoProjetoViewModel
            {
                Ativos = await _context.TipoProjetos
                    .Where(tp => tp.FlAtivo)
                    .ToListAsync(),

                Inativos = await _context.TipoProjetos
                    .Where(tp => !tp.FlAtivo)
                    .ToListAsync()
            };

            return View("~/Views/Home/TipoProjeto.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("CALL sp_ExcluirTipoProjeto({0})", id);
                TempData["Mensagem"] = "Tipo de projeto excluído com sucesso! Agora o Tipo de projeto está inativo.";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao excluir tipo de projeto: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(TbTipoProjeto tipo)
        {
            try
            {
                var existente = await _context.TipoProjetos.FindAsync(tipo.IdTipoProjeto);
                if (existente == null)
                {
                    TempData["Mensagem"] = "Tipo de projeto não encontrado!";
                    return RedirectToAction("Index");
                }

                existente.Descricao = tipo.Descricao;
                existente.FlAtivo = tipo.FlAtivo;

                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Tipo de projeto atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao atualizar tipo de projeto: {ex.Message}";
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> Inserir(TbTipoProjeto tipo)
        {
            try
            {
                _context.TipoProjetos.Add(tipo);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Tipo de projeto adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao adicionar tipo de projeto: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

    }
}
