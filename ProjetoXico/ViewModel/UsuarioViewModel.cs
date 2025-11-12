using System.Collections.Generic;

namespace ProjetoXico.Models
{
    public class UsuarioViewModel
    {
        public List<TbUsuario> Ativos { get; set; } = new();
        public List<TbUsuario> Inativos { get; set; } = new();
    }
}
