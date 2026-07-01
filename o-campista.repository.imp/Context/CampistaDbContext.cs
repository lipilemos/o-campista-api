
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
                .HasForeignKey(x => x.CampingId)
                .IsRequired(false);

            modelBuilder.Entity<Checkin>()
                .HasOne(x => x.Trilha)
                .WithMany()
                .HasForeignKey(x => x.TrilhaId)
                .IsRequired(false);

            modelBuilder.Entity<Trilha>()
                .HasOne(t => t.Camping)
                .WithMany(c => c.Trilhas)
                .HasForeignKey(t => t.CampingId)
                .IsRequired(false);

            modelBuilder.Entity<CampingAvaliacao>()
                .HasOne(a => a.Camping)
                .WithMany(c => c.Avaliacoes)
                .HasForeignKey(a => a.CampingId)
                .IsRequired(false);

            modelBuilder.Entity<CampingAvaliacao>()
                .HasOne(a => a.Trilha)
                .WithMany()
                .HasForeignKey(a => a.TrilhaId)
                .IsRequired(false);

            modelBuilder.Entity<MensagemChat>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId);

            modelBuilder.Entity<MensagemChat>()
                .HasOne(m => m.Camping)
                .WithMany()
                .HasForeignKey(m => m.CampingId);

            modelBuilder.Entity<MensagemChat>()
                .HasIndex(m => new { m.CampingId, m.DataEnvio })
                .HasDatabaseName("IX_MensagensChat_CampingId_DataEnvio");

            modelBuilder.HasDbFunction(typeof(CampistaDbContext)
            .GetMethod(nameof(BuscarPresentesProximos), new[] { typeof(double), typeof(double), typeof(double) })!)
            .HasName("buscar_presentes_proximos"); // Nome exato criado no SQL

            modelBuilder.Entity<SalaChatMembro>()
                .HasKey(x => new { x.SalaId, x.UsuarioId });

            modelBuilder.Entity<SalaChat>()
                .HasOne(s => s.Camping)
                .WithMany()
                .HasForeignKey(s => s.CampingId);

            modelBuilder.Entity<SalaChat>()
                .HasOne(s => s.CriadoPor)
                .WithMany()
                .HasForeignKey(s => s.CriadoPorId);

            modelBuilder.Entity<SalaChatMembro>()
                .HasOne(m => m.Sala)
                .WithMany(s => s.Membros)
                .HasForeignKey(m => m.SalaId);

            modelBuilder.Entity<SalaChatMembro>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId);

            modelBuilder.Entity<MensagemSalaChat>()
                .HasOne(m => m.Sala)
                .WithMany(s => s.Mensagens)
                .HasForeignKey(m => m.SalaId);

            modelBuilder.Entity<MensagemSalaChat>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId);

            modelBuilder.Entity<MensagemSalaChat>()
                .HasIndex(m => new { m.SalaId, m.DataEnvio })
                .HasDatabaseName("IX_MensagensSalaChat_SalaId_DataEnvio");

            modelBuilder.Entity<SalaChat>()
                .HasIndex(s => s.CampingId)
                .HasDatabaseName("IX_SalaChat_CampingId");

            modelBuilder.Entity<SalaChat>()
                .HasIndex(s => s.CodigoConvite)
                .IsUnique()
                .HasFilter("codigo_convite IS NOT NULL")
                .HasDatabaseName("IX_SalaChat_CodigoConvite");

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
        public DbSet<CampingAvaliacao> CampingAvaliacoes { get; set; }
        public DbSet<MensagemChat> MensagensChat { get; set; }
        public DbSet<SalaChat> SalasChat { get; set; }
        public DbSet<SalaChatMembro> SalaChatMembros { get; set; }
        public DbSet<MensagemSalaChat> MensagensSalaChat { get; set; }
    }
}
