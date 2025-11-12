using ProjetoXico.Models;
using System.Collections.Generic;

namespace ProjetoXico.ViewModel
{
    public class ProjetoMaterialViewModel
    {
        public TbProjeto Projeto { get; set; } = new();
        public List<TbProjetoMaterial> MateriaisVinculados { get; set; } = new();
        public List<TbMaterial> MateriaisDisponiveis { get; set; } = new();
    }
}
