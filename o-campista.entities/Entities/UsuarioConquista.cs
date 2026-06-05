using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_usuario_conquista")]
public class UsuarioConquista
{
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("conquista_id")]
    public long ConquistaId { get; set; }

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public virtual Usuario Usuario { get; set; } = null!;

    [ForeignKey(nameof(ConquistaId))]
    public virtual Conquista Conquista { get; set; } = null!;
}
