
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

            modelBuilder.Entity<Seguidor>()
                .HasKey(s => new { s.SeguidorId, s.SeguidoId });

            modelBuilder.Entity<Seguidor>()
                .HasOne(s => s.SeguidorUsuario)
                .WithMany(u => u.Seguindo)
                .HasForeignKey(s => s.SeguidorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Seguidor>()
                .HasOne(s => s.SeguidoUsuario)
                .WithMany(u => u.Seguidores)
                .HasForeignKey(s => s.SeguidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConfiguracaoPrivacidade>()
                .HasOne(c => c.Usuario)
                .WithOne(u => u.Privacidade)
                .HasForeignKey<ConfiguracaoPrivacidade>(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CurtidaPost>()
                .HasKey(c => new { c.PostId, c.UsuarioId });

            modelBuilder.Entity<CurtidaPost>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Curtidas)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CurtidaPost>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComentarioPost>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comentarios)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComentarioPost>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ComentarioPost>()
                .HasIndex(c => new { c.PostId, c.CriadoEm })
                .HasDatabaseName("IX_ComentariosPost_PostId_CriadoEm");

            modelBuilder.Entity<PostViagem>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostViagem>()
                .HasOne(p => p.Camping)
                .WithMany()
                .HasForeignKey(p => p.CampingId)
                .IsRequired(false);

            modelBuilder.Entity<PostViagem>()
                .HasOne(p => p.Trilha)
                .WithMany()
                .HasForeignKey(p => p.TrilhaId)
                .IsRequired(false);

            modelBuilder.Entity<AtividadeFeed>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AtividadeFeed>()
                .HasIndex(a => new { a.UsuarioId, a.CriadoEm })
                .HasDatabaseName("IX_AtividadeFeed_UsuarioId_CriadoEm");

            modelBuilder.Entity<AtividadeFeed>()
                .HasIndex(a => a.CriadoEm)
                .HasDatabaseName("IX_AtividadeFeed_CriadoEm");

            modelBuilder.Entity<Notificacao>()
                .HasOne(n => n.Destinatario)
                .WithMany()
                .HasForeignKey(n => n.DestinatarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notificacao>()
                .HasOne(n => n.Remetente)
                .WithMany()
                .HasForeignKey(n => n.RemetenteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notificacao>()
                .HasIndex(n => new { n.DestinatarioId, n.CriadoEm })
                .HasDatabaseName("IX_Notificacao_DestinatarioId_CriadoEm");

            modelBuilder.Entity<Notificacao>()
                .HasIndex(n => new { n.DestinatarioId, n.Lida })
                .HasDatabaseName("IX_Notificacao_DestinatarioId_Lida");

            modelBuilder.Entity<UsuarioCampingFavorito>()
                .HasKey(f => new { f.UsuarioId, f.CampingId });

            modelBuilder.Entity<UsuarioCampingFavorito>()
                .HasOne(f => f.Usuario)
                .WithMany()
                .HasForeignKey(f => f.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioCampingFavorito>()
                .HasOne(f => f.Camping)
                .WithMany()
                .HasForeignKey(f => f.CampingId)
                .OnDelete(DeleteBehavior.Cascade);

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
        public DbSet<Seguidor> Seguidores { get; set; }
        public DbSet<ConfiguracaoPrivacidade> ConfiguracaoPrivacidades { get; set; }
        public DbSet<PostViagem> PostsViagem { get; set; }
        public DbSet<CurtidaPost> CurtidasPost { get; set; }
        public DbSet<ComentarioPost> ComentariosPost { get; set; }
        public DbSet<AtividadeFeed> AtividadesFeed { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<UsuarioCampingFavorito> UsuarioCampingFavoritos { get; set; }
    }
}
