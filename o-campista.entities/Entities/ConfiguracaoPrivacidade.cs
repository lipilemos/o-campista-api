using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace o_campista.entities.Entities;

[Table("tb_configuracao_privacidade")]
public class ConfiguracaoPrivacidade
{
    [Key]
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public virtual Usuario Usuario { get; set; } = null!;

    [Column("perfil_publico")]
    public bool PerfilPublico { get; set; } = true;

    [Column("checkins_publicos")]
    public bool CheckinsPublicos { get; set; } = true;

    [Column("conquistas_publicas")]
    public bool ConquistasPublicas { get; set; } = true;

    [Column("nivel_publico")]
    public bool NivelPublico { get; set; } = true;
}
