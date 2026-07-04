using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_post_viagem")]
public class PostViagem
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("texto")]
    [MaxLength(1000)]
    public string Texto { get; set; } = string.Empty;

    [Column("foto_url")]
    public string? FotoUrl { get; set; }

    [Column("camping_id")]
    public long? CampingId { get; set; }

    [Column("trilha_id")]
    public long? TrilhaId { get; set; }

    [Column("latitude")]
    public decimal? Latitude { get; set; }

    [Column("longitude")]
    public decimal? Longitude { get; set; }

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public virtual Usuario Usuario { get; set; } = null!;

    [ForeignKey(nameof(CampingId))]
    public virtual Camping? Camping { get; set; }

    [ForeignKey(nameof(TrilhaId))]
    public virtual Trilha? Trilha { get; set; }

    public virtual ICollection<CurtidaPost> Curtidas { get; set; } = [];
    public virtual ICollection<ComentarioPost> Comentarios { get; set; } = [];
}
