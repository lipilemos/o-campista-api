using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities
{
    [Table("tb_camping_avaliacao")]
    public class CampingAvaliacao
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("camping_id")]
        public long? CampingId { get; set; }

        [Column("trilha_id")]
        public long? TrilhaId { get; set; }

        [Column("usuario_id")]
        public Guid UsuarioId { get; set; }

        [Column("checkin_id")]
        public long CheckinId { get; set; }

        [Column("nota")]
        [Range(1, 5)]
        public int Nota { get; set; }

        [Column("comentario")]
        [MaxLength(1000)]
        public string Comentario { get; set; } = string.Empty;

        [Column("foto_url")]
        public string? FotoUrl { get; set; }

        [Column("criado_em")]
        public DateTime CriadoEm { get; set; }

        [ForeignKey(nameof(CampingId))]
        public virtual Camping? Camping { get; set; }

        [ForeignKey(nameof(TrilhaId))]
        public virtual Trilha? Trilha { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario Usuario { get; set; } = null!;

        [ForeignKey(nameof(CheckinId))]
        public virtual Checkin? Checkin { get; set; }
    }
}
