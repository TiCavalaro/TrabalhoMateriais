using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoXico.Models
{
    [Table("TB_PROJETO_MATERIAIS")]
    public class TbProjetoMaterial
    {
        [Key]
        [Column("ID_PROJETO_MATERIAL")]
        public int IdProjetoMaterial { get; set; }

        [Required]
        [Column("ID_PROJETO")]
        public int IdProjeto { get; set; }

        [Required]
        [Column("ID_MATERIAL")]
        public int IdMaterial { get; set; }

        [Required]
        [Column("QUANTIDADE_UTILIZADA")]
        public decimal QuantidadeUtilizada { get; set; }

        [Required]
        [Column("ID_COLABORADOR_INSERCAO")]
        public int IdColaboradorInsercao { get; set; }

        [Column("DATA_INSERCAO")]
        public DateTime DataInsercao { get; set; } = DateTime.Now;

        [Column("FL_EXCLUIDO")]
        public bool FlExcluido { get; set; } = false;

        [Column("FL_ADICIONAL")]
        public bool FlAdicional { get; set; } = false;

        [ForeignKey("IdProjeto")]
        public TbProjeto? Projeto { get; set; }

        [ForeignKey("IdMaterial")]
        public TbMaterial? Material { get; set; }
    }
}
