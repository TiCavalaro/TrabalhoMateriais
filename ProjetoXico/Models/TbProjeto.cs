using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoXico.Models
{
    [Table("TB_PROJETO")]
    public class TbProjeto
    {
        [Key]
        [Column("ID_PROJETO")]
        public int IdProjeto { get; set; }

        [Required]
        [Column("NOME_PROJETO")]
        [MaxLength(200)]
        public string NomeProjeto { get; set; } = string.Empty;

        [Column("DATA_PLANEJADA_INICIO")]
        public DateTime DataPlanejadaInicio { get; set; }

        [Column("DATA_PLANEJADA_FIM")]
        public DateTime DataPlanejadaFim { get; set; }

        [Column("TIPO_PROJETO")]
        public int TipoProjeto { get; set; }

        [Required]
        [Column("ID_COLABORADOR_CRIACAO")]
        public int IdColaboradorCriacao { get; set; }

        [Required]
        [Column("DATA_CRIACAO")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [Column("DATA_FINALIZACAO")]
        public DateTime? DataFinalizacao { get; set; }

        [Column("FL_FINALIZADO")]
        public bool FlFinalizado { get; set; } = false;

        [Column("FL_ATIVO")]
        public bool FlAtivo { get; set; } = true;

        public ICollection<TbProjetoMaterial>? ProjetoMateriais { get; set; }
    }
}
