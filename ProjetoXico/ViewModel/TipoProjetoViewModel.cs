using System.Collections.Generic;

namespace ProjetoXico.Models
{
    public class TipoProjetoViewModel
    {
        public List<TbTipoProjeto> Ativos { get; set; } = new List<TbTipoProjeto>();
        public List<TbTipoProjeto> Inativos { get; set; } = new List<TbTipoProjeto>();
    }
}
