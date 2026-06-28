
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace o_campista.entities.Entities
    {
        [Table("tb_usuario_trilha")]
        public class UsuarioTrilha
        {
            [Key]
            [Column("id")]
            public long Id { get; set; }

            [Column("usuario_id")]
            public Guid UsuarioId { get; set; }

            [ForeignKey(nameof(UsuarioId))]
            public virtual Usuario Usuario { get; set; } = null!;

            [Column("trilha_id")]
            public long TrilhaId { get; set; }

            [ForeignKey(nameof(TrilhaId))]
            public virtual Trilha Trilha { get; set; } = null!;

            [Column("concluida")]
            public bool Concluida { get; set; }

            [Column("criado_em")]
            public DateTime CriadoEm { get; set; }

            [Column("concluida_em")]
            public DateTime? ConcluidaEm { get; set; }
        }
    }
