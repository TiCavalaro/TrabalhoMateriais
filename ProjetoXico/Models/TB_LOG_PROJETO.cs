using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoXico.Models
{
    [Table("TB_LOG_PROJETO")]
    public class TbLogProjeto
    {
        [Key]
        [Column("ID_LOG")]
        public int IdLog { get; set; }

        [Column("ID_PROJETO")]
        public int IdProjeto { get; set; }

        [Column("ID_USUARIO")]
        public int? IdUsuario { get; set; }

        [Column("OPERACAO")]
        public string Operacao { get; set; } = string.Empty;

        [Column("DATA_OPERACAO")]
        public DateTime DataOperacao { get; set; }

        [Column("DESCRICAO")]
        public string? Descricao { get; set; }
    }
}
