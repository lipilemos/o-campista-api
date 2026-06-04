using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace o_campista.entities.Entities
{
    [Table("tb_camping_fotos")]
    public class CampingFoto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("camping_id")]
        public long CampingId { get; set; }

        [Column("url")]
        public string Url { get; set; } = string.Empty;

        [Column("principal")]
        public bool Principal { get; set; }

        [Column("ordem")]
        public int Ordem { get; set; }

        [Column("criado_em")]
        public DateTime CriadoEm { get; set; }

        [ForeignKey(nameof(CampingId))]
        public virtual Camping Camping { get; set; } = null!;
    }
}
