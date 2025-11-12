using Microsoft.EntityFrameworkCore;
using ProjetoXico.Models;

namespace ProjetoXico.Data
{
    public class BancoContext : DbContext
    {
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }

        public DbSet<TbTipoProjeto> TipoProjetos { get; set; }

        public DbSet<TbProjeto> Projetos { get; set; }

        public DbSet<TbMaterial> Materiais { get; set; }

        public DbSet<TbUsuario> Usuarios { get; set; }

        public DbSet<TbProjetoMaterial> ProjetoMateriais { get; set; }

        public DbSet<TbLogProjeto> LogsProjeto { get; set; }



    }
}
