using ProjetoXico.Models;
using System.Collections.Generic;


namespace ProjetoXico.ViewModel
{
    public class MaterialViewModel
    {
        public List<TbMaterial> Ativos { get; set; } = new List<TbMaterial>();
        public List<TbMaterial> Inativos { get; set; } = new List<TbMaterial>();
    }
}



