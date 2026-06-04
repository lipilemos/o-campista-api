using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace o_campista.entities.Entities
{
    [Table("tb_camping_recurso")]
    public class CampingRecurso
    {
        [Column("camping_id")]
        public long CampingId { get; set; }

        [Column("recurso_id")]
        public long RecursoId { get; set; }

        [Column("disponivel")]
        public bool Disponivel { get; set; }

        [ForeignKey(nameof(CampingId))]
        public virtual Camping Camping { get; set; } = null!;

        [ForeignKey(nameof(RecursoId))]
        public virtual Recurso Recurso { get; set; } = null!;
    }
}
