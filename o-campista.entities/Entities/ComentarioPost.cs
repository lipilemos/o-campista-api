using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_comentario_post")]
public class ComentarioPost
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("post_id")]
    public long PostId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("texto")]
    [MaxLength(500)]
    public string Texto { get; set; } = string.Empty;

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }

    [ForeignKey(nameof(PostId))]
    public virtual PostViagem Post { get; set; } = null!;

    [ForeignKey(nameof(UsuarioId))]
    public virtual Usuario Usuario { get; set; } = null!;
}
