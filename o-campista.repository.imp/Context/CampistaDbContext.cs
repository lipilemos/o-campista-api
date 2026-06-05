
using Microsoft.EntityFrameworkCore;
using o_campista.entities.Entities;

namespace o_campista.api.Context
{
    public class CampistaDbContext : DbContext
    {
        public CampistaDbContext(
            DbContextOptions<CampistaDbContext> options
        ) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CampingRecurso>()
                .HasKey(x => new
                {
                    x.CampingId,
                    x.RecursoId
                });
            modelBuilder.Entity<UsuarioConquista>()
                .HasKey(x => new
                {
                    x.UsuarioId,
                    x.ConquistaId
                });

            modelBuilder.Entity<UsuarioPresente>()
                .HasKey(x => new
                {
                    x.UsuarioId,
                    x.PresenteId
                });

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Camping> Campings { get; set; }
        public DbSet<CampingFoto> CampingFotos { get; set; }
        public DbSet<CampingRecurso> CampingRecursos { get; set; }
        public DbSet<Recurso> Recursos { get; set; }
        public DbSet<Trilha> Trilhas { get; set; }
        public DbSet<TrilhaPonto> TrilhaPontos { get; set; }
        public DbSet<Conquista> Conquistas { get; set; }
        public DbSet<Presente> Presentes { get; set; }
        public DbSet<UsuarioConquista> UsuarioConquistas { get; set; }
        public DbSet<UsuarioPresente> UsuarioPresentes { get; set; }
    }
}
