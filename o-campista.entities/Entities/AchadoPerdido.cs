using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_achado_perdido")]
public class AchadoPerdido
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("camping_id")]
    public long CampingId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    /// <summary>"achado" (encontrei este item) ou "perdido" (perdi este item).</summary>
    [Column("tipo")]
    [MaxLength(10)]
    public string Tipo { get; set; } = string.Empty;

    [Column("titulo")]
    [MaxLength(120)]
    public string Titulo { get; set; } = string.Empty;

    [Column("descricao")]
    [MaxLength(500)]
    public string? Descricao { get; set; }

    [Column("foto_url")]
    public string? FotoUrl { get; set; }

    /// <summary>Onde o item está guardado, ex.: "Recepção".</summary>
    [Column("local_guarda")]
    [MaxLength(120)]
    public string? LocalGuarda { get; set; }

    [Column("resolvido")]
    public bool Resolvido { get; set; }

    [Column("resolvido_em")]
    public DateTime? ResolvidoEm { get; set; }

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public virtual Usuario Usuario { get; set; } = null!;

    [ForeignKey(nameof(CampingId))]
    public virtual Camping Camping { get; set; } = null!;
}
