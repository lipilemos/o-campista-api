using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace o_campista.entities.Entities
{
    [Table("tb_conquista")]
    public class Conquista
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [Column("icone")]
        public string? Icone { get; set; }

        [Column("xp_bonus")]
        public int XpBonus { get; set; }

        public virtual ICollection<UsuarioConquista> Usuarios { get; set; } = [];
    }
}
