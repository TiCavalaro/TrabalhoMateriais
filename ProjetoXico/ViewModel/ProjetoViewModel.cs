using ProjetoXico.Models;
using System.Collections.Generic;

namespace ProjetoXico.ViewModel
{
    public class ProjetoViewModel
    {
        public List<TbProjeto> Ativos { get; set; } = new List<TbProjeto>();
        public List<TbProjeto> Inativos { get; set; } = new List<TbProjeto>();
        public List<TbTipoProjeto> TiposProjeto { get; set; } = new(); 

    }
}
