using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoXico.Models
{
    [Table("TB_TIPO_PROJETO")]
    public class TbTipoProjeto
    {
        [Key]
        [Column("ID_TIPO_PROJETO")]
        public int IdTipoProjeto { get; set; }

        [Required]
        [Column("DESCRICAO")]
        [MaxLength(100)]
        public string Descricao { get; set; }

        [Column("FL_ATIVO")]
        public bool FlAtivo { get; set; } = true;
    }
}
