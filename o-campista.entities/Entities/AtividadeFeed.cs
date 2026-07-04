using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_atividade_feed")]
public class AtividadeFeed
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    /// <summary>checkin | conquista | avaliacao | post | trilha_concluida</summary>
    [Column("tipo")]
    [MaxLength(30)]
    public string Tipo { get; set; } = string.Empty;

    [Column("referencia_id")]
    public long ReferenciaId { get; set; }

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }

    [Column("visivel")]
    public bool Visivel { get; set; } = true;

    [ForeignKey(nameof(UsuarioId))]
    public virtual Usuario Usuario { get; set; } = null!;
}
