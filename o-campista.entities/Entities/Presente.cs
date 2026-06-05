using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace o_campista.entities.Entities
{
    [Table("tb_presente")]
    public class Presente
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [Column("codigo_resgate")]
        public string? CodigoResgate { get; set; }

        public virtual ICollection<UsuarioPresente> Usuarios { get; set; } = [];
    }
}
