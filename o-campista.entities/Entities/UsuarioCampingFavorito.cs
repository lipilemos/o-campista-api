using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_usuario_camping_favorito")]
public class UsuarioCampingFavorito
{
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public virtual Usuario Usuario { get; set; } = null!;

    [Column("camping_id")]
    public long CampingId { get; set; }

    [ForeignKey(nameof(CampingId))]
    public virtual Camping Camping { get; set; } = null!;

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }
}
