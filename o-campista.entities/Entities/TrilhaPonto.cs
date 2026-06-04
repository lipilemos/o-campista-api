using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace o_campista.entities.Entities
{
    [Table("tb_trilha_pontos")]
    public class TrilhaPonto
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("trilha_id")]
        public long TrilhaId { get; set; }

        [Column("ordem")]
        public int Ordem { get; set; }

        [Column("latitude")]
        public decimal Latitude { get; set; }

        [Column("longitude")]
        public decimal Longitude { get; set; }

        [ForeignKey(nameof(TrilhaId))]
        public virtual Trilha Trilha { get; set; } = null!;
    }
}
