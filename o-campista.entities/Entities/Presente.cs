using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace o_campista.entities.Entities
{
    [Table("tb_presente")]
    public class Presente
    {
        [Key]
        [Column("id")]
        public long Id { get; set; } // int8 mapeia para long

        [Column("nome")]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("foto_url")]
        public string FotoUrl { get; set; } = string.Empty;

        [Column("location")]
        public Point Location { get; set; } = default!; // Tipo geográfico do PostGIS

        [Column("codigo_resgate")]
        public string? CodigoResgate { get; set; }

        [Column("usuario_criador_id")]
        public Guid UsuarioCriadorId { get; set; }

        [Column("esta_disponivel")]
        public bool? EstaDisponivel { get; set; } = true;

        [Column("criado_em")]
        public DateTime? CriadoEm { get; set; } = DateTime.UtcNow;

        // Relacionamentos
        [ForeignKey("UsuarioCriadorId")]
        public virtual Usuario? Criador { get; set; }

        public virtual ICollection<UsuarioPresente> Usuarios { get; set; } = new List<UsuarioPresente>();
    }
}
