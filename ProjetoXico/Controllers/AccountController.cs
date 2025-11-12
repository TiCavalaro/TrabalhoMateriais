using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoXico.Data;
using ProjetoXico.Models;
using System.Security.Cryptography;
using System.Text;

namespace ProjetoXico.Controllers
{
    public class AccountController : Controller
    {
        private readonly BancoContext _context;

        public AccountController(BancoContext context)
        {
            _context = context;
        }

        // 🔐 Exibe a página de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 🔐 Autentica o usuário
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Preencha todos os campos.";
                return View();
            }

            // Gera o hash SHA-1 da senha informada
            string passwordHash = GerarSha1(password);

            var usuario = await _context.Set<TbUsuario>()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (usuario == null)
            {
                ViewBag.Error = "Usuário não encontrado.";
                return View();
            }

            if (!usuario.FlAtivo)
            {
                ViewBag.Error = "Conta inativa. Contate o administrador.";
                return View();
            }

            if (usuario.Bloqueado)
            {
                ViewBag.Error = "Conta bloqueada por excesso de tentativas.";
                return View();
            }

            // Valida a senha
            if (usuario.SenhaHash != passwordHash)
            {
                usuario.TentativasFalhas++;

                if (usuario.TentativasFalhas >= 3)
                {
                    usuario.Bloqueado = true;
                    ViewBag.Error = "Conta bloqueada por excesso de tentativas.";
                }
                else
                {
                    ViewBag.Error = "Senha incorreta.";
                }

                await _context.SaveChangesAsync();
                return View();
            }

            // Login bem-sucedido
            usuario.TentativasFalhas = 0;
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("user_name", usuario.Username);
            HttpContext.Session.SetInt32("user_id", usuario.Id);

            return RedirectToAction("Index", "Home");
        }

        // 🚪 Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // 🧍 Cadastro de novo usuário
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Preencha todos os campos.";
                return View();
            }

            bool usuarioExiste = await _context.Set<TbUsuario>()
                .AnyAsync(u => u.Username == username);

            if (usuarioExiste)
            {
                ViewBag.Error = "Esse nome de usuário já está em uso.";
                return View();
            }

            var novoUsuario = new TbUsuario
            {
                Username = username,
                Email = email,
                SenhaHash = GerarSha1(password),
                Bloqueado = false,
                TentativasFalhas = 0,
                FlAtivo = true
            };

            _context.Add(novoUsuario);
            await _context.SaveChangesAsync();

            ViewBag.Success = "Conta criada com sucesso! Faça login para continuar.";
            return RedirectToAction("Login");
        }

        // 🔐 Gera o hash SHA-1 (compatível com o banco atual)
        private static string GerarSha1(string input)
        {
            using var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
