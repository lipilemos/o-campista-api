using o_campista.entities.Entities.o_campista.entities.Entities;
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
    [Column("foto_perfil")]
    public string? FotoPerfil { get; set; }
    public virtual ICollection<UsuarioConquista> UsuarioConquistas { get; set; } = [];
    public virtual ICollection<UsuarioPresente> UsuarioPresentes { get; set; } = [];
    public virtual ICollection<UsuarioTrilha> UsuarioTrilhas { get; set; } = [];
    public virtual ICollection<Checkin> Checkins { get; set; } = [];
    public virtual ICollection<CampingAvaliacao> Avaliacoes { get; set; } = [];
}
