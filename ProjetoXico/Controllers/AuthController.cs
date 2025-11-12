using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using ProjetoXico.Data;
using ProjetoXico.Models;
using System.Security.Claims;

namespace ProjetoXico.Controllers
{
    public class AuthController : Controller
    {
        private readonly BancoContext _context;

        public AuthController(BancoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string senha)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Erro = "Preencha todos os campos!";
                return View();
            }

            try
            {
                var resultado = await _context.Usuarios
                    .FromSqlRaw("CALL sp_ValidaLogin({0}, {1})", login, senha)
                    .ToListAsync();

                if (resultado == null || !resultado.Any())
                {
                    ViewBag.Erro = "Usuário ou senha inválidos!";
                    return View();
                }

                var usuario = resultado.First();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Username),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException ex)
            {
                ViewBag.Erro = "Erro de banco de dados: " + ex.InnerException?.Message;
                return View();
            }
            catch (MySqlException ex)
            {
                // Captura a mensagem do SIGNAL SQLSTATE (ex: "Senha incorreta! Tentativas restantes: 1")
                ViewBag.Erro = ex.Message;
                return View();
            }
            catch (Exception)
            {
                ViewBag.Erro = "Erro ao validar login. Tente novamente.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public IActionResult TrocarSenha()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> TrocarSenha(string novaSenha, string confirmarSenha)
        {
            if (string.IsNullOrWhiteSpace(novaSenha) || string.IsNullOrWhiteSpace(confirmarSenha))
            {
                ViewBag.Erro = "Preencha todos os campos!";
                return View();
            }

            if (novaSenha != confirmarSenha)
            {
                ViewBag.Erro = "As senhas não coincidem!";
                return View();
            }

            try
            {
                var usuarioLogado = User.Identity?.Name;

                if (string.IsNullOrEmpty(usuarioLogado))
                {
                    ViewBag.Erro = "Sessão expirada. Faça login novamente.";
                    return RedirectToAction("Login");
                }

                int linhasAfetadas = await _context.Database.ExecuteSqlRawAsync(
                    "CALL sp_TrocarSenha({0}, {1})", usuarioLogado, novaSenha);

                if (linhasAfetadas == 0)
                {
                    ViewBag.Erro = "Não foi possível alterar a senha.";
                    return View();
                }

                TempData["Sucesso"] = "Senha alterada com sucesso!";
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                ViewBag.Erro = "Erro ao alterar a senha. Tente novamente.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DesbloquearUsuario(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("CALL sp_DesbloquearUsuario({0})", id);
                TempData["Mensagem"] = "Usuário desbloqueado com sucesso!";
            }
            catch
            {
                TempData["MensagemErro"] = "Erro ao desbloquear o usuário.";
            }

            return RedirectToAction("Login");
        }
    }
}
