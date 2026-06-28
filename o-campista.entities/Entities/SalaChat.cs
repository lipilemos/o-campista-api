using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_sala_chat")]
public class SalaChat
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("nome")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Column("tipo")]
    [MaxLength(20)]
    public string Tipo { get; set; } = string.Empty;

    [Column("camping_id")]
    public long? CampingId { get; set; }

    [Column("foto_capa")]
    public string? FotoCapa { get; set; }

    [Column("codigo_convite")]
    [MaxLength(8)]
    public string? CodigoConvite { get; set; }

    [Column("criado_por_id")]
    public Guid? CriadoPorId { get; set; }

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }

    public Camping? Camping { get; set; }
    public Usuario? CriadoPor { get; set; }
    public virtual ICollection<SalaChatMembro> Membros { get; set; } = [];
    public virtual ICollection<MensagemSalaChat> Mensagens { get; set; } = [];
}
