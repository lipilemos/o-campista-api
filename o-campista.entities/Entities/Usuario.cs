using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;


[Table("tb_usuario")]
public class Usuario
{
    [Column("id")]
    public Guid Id { get; set; }
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    [Column("senha_hash")]
    public string SenhaHash { get; set; } = string.Empty;
    [Column("data_criacao")]
    public DateTime DataCriacao { get; set; }
    [Column("ativo")]
    public bool Ativo { get; set; } 
    [Column("nivel")]
    public int Nivel { get; set; } 
    [Column("xp")]
    public int XP { get; set; }
    public virtual ICollection<UsuarioConquista> UsuarioConquistas { get; set; } = [];

    public virtual ICollection<UsuarioPresente> UsuarioPresentes { get; set; } = [];
}
