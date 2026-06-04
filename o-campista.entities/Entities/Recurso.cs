using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace o_campista.entities.Entities;

[Table("tb_recurso")]
public class Recurso
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("nome")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    public virtual ICollection<CampingRecurso> Campings { get; set; } = [];
}