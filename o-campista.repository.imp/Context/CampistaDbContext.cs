
using Microsoft.EntityFrameworkCore;
using o_campista.entities.Entities;
using o_campista.entities.Entities.o_campista.entities.Entities;


namespace o_campista.api.Context
{
    public class CampistaDbContext : DbContext
    {
        public CampistaDbContext(
            DbContextOptions<CampistaDbContext> options
        ) : base(options)
        {

        }
        public IQueryable<Presente> BuscarPresentesProximos(double lat, double lon, double raio)
        => FromExpression(() => BuscarPresentesProximos(lat, lon, raio));
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
            modelBuilder.Entity<Presente>()
                .Property(x => x.Location)
                .HasColumnType("geography(Point,4326)");
            
            modelBuilder.Entity<Checkin>()
                .HasOne(x => x.Usuario)
                .WithMany(x => x.Checkins)
                .HasForeignKey(x => x.UsuarioId);

            modelBuilder.Entity<Checkin>()
                .HasOne(x => x.Camping)
                .WithMany()
                .HasForeignKey(x => x.CampingId);

            modelBuilder.HasDbFunction(typeof(CampistaDbContext)
            .GetMethod(nameof(BuscarPresentesProximos), new[] { typeof(double), typeof(double), typeof(double) })!)
            .HasName("buscar_presentes_proximos"); // Nome exato criado no SQL

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
        public DbSet<UsuarioTrilha> UsuarioTrilhas{ get; set; }
        public DbSet<Checkin> Checkins { get; set; }
    }
}
