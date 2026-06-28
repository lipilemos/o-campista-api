using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities
{
    [Table("tb_mensagem_chat")]
    public class MensagemChat
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("camping_id")]
        public long CampingId { get; set; }

        [Column("usuario_id")]
        public Guid UsuarioId { get; set; }

        [Column("texto")]
        [MaxLength(500)]
        public string Texto { get; set; } = null!;

        [Column("data_envio")]
        public DateTime DataEnvio { get; set; }

        public Camping Camping { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}
