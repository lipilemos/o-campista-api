using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_mensagem_sala_chat")]
public class MensagemSalaChat
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("sala_id")]
    public long SalaId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("texto")]
    [MaxLength(500)]
    public string Texto { get; set; } = null!;

    [Column("data_envio")]
    public DateTime DataEnvio { get; set; }

    public SalaChat Sala { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
