using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoXico.Models
{
    [Table("TB_MATERIAIS")] 
    public class TbMaterial
    {
        [Key]
        [Column("ID_MATERIAL")]
        public int IdMaterial { get; set; }

        [Required]
        [Column("DESCRICAO")]
        [MaxLength(200)]
        public string Descricao { get; set; }

        [Required]
        [Column("UNIDADE_MEDIDA")]
        [MaxLength(50)]
        public string UnidadeMedida { get; set; }

        [Column("QUANTIDADE", TypeName = "decimal(18,2)")]
        public decimal Quantidade { get; set; } = 0;

        [Column("METRICA")]
        [MaxLength(50)]
        public string? Metrica { get; set; }

        [Column("VALOR_UNITARIO", TypeName = "decimal(18,2)")]
        public decimal? ValorUnitario { get; set; }

        [Required]
        [Column("ID_COLABORADOR_INSERCAO")]
        public int IdColaboradorInsercao { get; set; }

        [Required]
        [Column("DATA_INSERCAO")]
        public DateTime DataInsercao { get; set; } = DateTime.Now;

        [Column("FL_ATIVO")]
        public bool FlAtivo { get; set; } = true;

        public ICollection<TbProjetoMaterial>? ProjetoMateriais { get; set; }
    }
}
