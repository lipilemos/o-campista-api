using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_curtida_post")]
public class CurtidaPost
{
    [Column("post_id")]
    public long PostId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }

    [ForeignKey(nameof(PostId))]
    public virtual PostViagem Post { get; set; } = null!;

    [ForeignKey(nameof(UsuarioId))]
    public virtual Usuario Usuario { get; set; } = null!;
}
