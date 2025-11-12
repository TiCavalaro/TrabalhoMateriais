using Microsoft.EntityFrameworkCore;
using ProjetoXico.Data;
using ProjetoXico.Models;

namespace ProjetoXico.Services
{
    public class AuthService
    {
        private readonly BancoContext _context;

        public AuthService(BancoContext context)
        {
            _context = context;
        }

        // Login do usuário
        public async Task<TbUsuario?> LoginAsync(string usernameOrEmail)
        {
            // Procura usuário por username ou email
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

            if (usuario == null) return null; // usuário não existe

            if (usuario.Bloqueado) return null; // usuário bloqueado

            return usuario;
        }

        // Incrementa tentativas falhas e bloqueia se necessário
        public async Task<bool> RegistrarFalhaAsync(TbUsuario usuario, int limiteTentativas = 3)
        {
            usuario.TentativasFalhas++;

            if (usuario.TentativasFalhas >= limiteTentativas)
            {
                usuario.Bloqueado = true;
            }

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return usuario.Bloqueado;
        }

        // Reseta tentativas falhas após login correto
        public async Task ResetarTentativasAsync(TbUsuario usuario)
        {
            usuario.TentativasFalhas = 0;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
