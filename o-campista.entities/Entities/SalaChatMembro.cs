using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_sala_chat_membro")]
public class SalaChatMembro
{
    [Column("sala_id")]
    public long SalaId { get; set; }

    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [Column("entrada_em")]
    public DateTime EntradaEm { get; set; }

    [Column("ultima_mensagem_lida_id")]
    public long? UltimaMensagemLidaId { get; set; }

    public SalaChat Sala { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
