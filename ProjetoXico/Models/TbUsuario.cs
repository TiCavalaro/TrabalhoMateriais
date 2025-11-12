using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoXico.Models
{
    [Table("TB_USUARIO")]
    public class TbUsuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public int Id { get; set; }

        [Required]
        [Column("USERNAME")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("EMAIL")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("SENHA_HASH")]
        [MaxLength(255)]
        public string SenhaHash { get; set; } = string.Empty;

        [Column("BLOQUEADO")]
        public bool Bloqueado { get; set; } = false;

        [Column("TENTATIVAS_FALHAS")]
        public int TentativasFalhas { get; set; } = 0;

        [Column("FL_ATIVO")]
        public bool FlAtivo { get; set; } = true;
    }
}
