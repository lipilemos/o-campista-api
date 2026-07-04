using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_notificacao")]
public class Notificacao
{
    [Column("id")]
    public long Id { get; set; }

    [Column("destinatario_id")]
    public Guid DestinatarioId { get; set; }

    [ForeignKey(nameof(DestinatarioId))]
    public virtual Usuario Destinatario { get; set; } = null!;

    [Column("remetente_id")]
    public Guid RemetenteId { get; set; }

    [ForeignKey(nameof(RemetenteId))]
    public virtual Usuario Remetente { get; set; } = null!;

    [Column("tipo")]
    public string Tipo { get; set; } = string.Empty;

    [Column("lida")]
    public bool Lida { get; set; }

    [Column("post_id")]
    public long? PostId { get; set; }

    [Column("post_texto")]
    public string? PostTexto { get; set; }

    [Column("comentario_texto")]
    public string? ComentarioTexto { get; set; }

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }
}
