using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace o_campista.entities.Entities
{
    [Table("tb_checkin")]
    public class Checkin
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("usuario_id")]
        public Guid UsuarioId { get; set; }
        [Column("camping_id")]
        public long? CampingId { get; set; }

        [Column("trilha_id")]
        public long? TrilhaId { get; set; }

        [Column("latitude")]
        public decimal Latitude { get; set; }
        [Column("longitude")]
        public decimal Longitude { get; set; }
        [Column("xp_ganho")]
        public int XpGanho { get; set; }
        [Column("ocupacao")]
        [MaxLength(20)]
        public string? Ocupacao { get; set; }

        [Column("criado_em")]
        public DateTime CriadoEm { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public Camping? Camping { get; set; }
        public Trilha? Trilha { get; set; }
    }
}
