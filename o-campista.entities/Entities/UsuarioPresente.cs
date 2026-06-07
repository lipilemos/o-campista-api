using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace o_campista.entities.Entities
{
    [Table("tb_usuario_presente")]
    public class UsuarioPresente
    {
        [Column("usuario_id")]
        public Guid UsuarioId { get; set; }

        [Column("presente_id")]
        public long PresenteId { get; set; }

        [Column("utilizado")]
        public bool Utilizado { get; set; }

        [Column("criado_em")]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario Usuario { get; set; } = null!;

        [ForeignKey(nameof(PresenteId))]
        public virtual Presente Presente { get; set; } = null!;
    }
}
