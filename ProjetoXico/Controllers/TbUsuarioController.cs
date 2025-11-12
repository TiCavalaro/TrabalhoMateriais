using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoXico.Data;
using ProjetoXico.Models;
using System.Security.Cryptography;
using System.Text;

namespace ProjetoXico.Controllers
{
    [Authorize] 
    public class TbUsuarioController : Controller
    {
        private readonly BancoContext _context;

        public TbUsuarioController(BancoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Listagem
        public async Task<IActionResult> Index()
        {
            var viewModel = new UsuarioViewModel
            {
                Ativos = await _context.Usuarios
                    .Where(u => u.FlAtivo)
                    .ToListAsync(),

                Inativos = await _context.Usuarios
                    .Where(u => !u.FlAtivo)
                    .ToListAsync()
            };

            return View("~/Views/Home/Usuario.cshtml", viewModel);
        }

        // POST: Inserir novo usuário
        [HttpPost]
        public async Task<IActionResult> Inserir(TbUsuario usuario, string Senha)
        {
            try
            {
                if (!string.IsNullOrEmpty(Senha))
                {
                    using var sha1 = SHA1.Create();
                    var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(Senha));
                    usuario.SenhaHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Usuário adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao adicionar usuário: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Atualizar(TbUsuario usuario, string Senha)
        {
            try
            {
                var existente = await _context.Usuarios.FindAsync(usuario.Id);
                if (existente == null)
                {
                    TempData["Mensagem"] = "Usuário não encontrado!";
                    return RedirectToAction("Index");
                }

                existente.Username = usuario.Username;
                existente.Email = usuario.Email;
                existente.FlAtivo = usuario.FlAtivo;
                existente.Bloqueado = usuario.Bloqueado;
                existente.TentativasFalhas = usuario.TentativasFalhas;

                if (!string.IsNullOrEmpty(Senha))
                {
                    using var sha1 = SHA1.Create();
                    var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(Senha));
                    existente.SenhaHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }

                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Usuário atualizado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao atualizar usuário: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // POST: Desativar usuário
        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario != null)
                {
                    usuario.FlAtivo = false;
                    await _context.SaveChangesAsync();
                    TempData["Mensagem"] = "Usuário desativado com sucesso!";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao desativar usuário: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // POST: Desbloquear usuário via procedure
        [HttpPost]
        public async Task<IActionResult> Desbloquear(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_DesbloquearUsuario({0});", id
                );

                TempData["Mensagem"] = "Usuário desbloqueado com sucesso!";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao desbloquear usuário: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
