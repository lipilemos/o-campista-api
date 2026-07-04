using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_usuario_seguidor")]
public class Seguidor
{
    [Column("seguidor_id")]
    public Guid SeguidorId { get; set; }

    [ForeignKey(nameof(SeguidorId))]
    public virtual Usuario SeguidorUsuario { get; set; } = null!;

    [Column("seguido_id")]
    public Guid SeguidoId { get; set; }

    [ForeignKey(nameof(SeguidoId))]
    public virtual Usuario SeguidoUsuario { get; set; } = null!;

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }
}
