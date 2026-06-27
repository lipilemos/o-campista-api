using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace o_campista.entities.Entities
{
    [Table("tb_camping")]
    public class Camping
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("nome")]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("endereco")]
        [MaxLength(500)]
        public string? Endereco { get; set; }

        [Column("cidade")]
        [MaxLength(100)]
        public string? Cidade { get; set; }

        [Column("estado")]
        [MaxLength(2)]
        public string? Estado { get; set; }

        [Column("telefone")]
        [MaxLength(30)]
        public string? Telefone { get; set; }

        [Column("latitude")]
        public decimal Latitude { get; set; }

        [Column("longitude")]
        public decimal Longitude { get; set; }

        [Column("tipo")]
        [MaxLength(50)]
        public string Tipo { get; set; } = string.Empty;

        [Column("avaliacao_media")]
        public decimal AvaliacaoMedia { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }

        [Column("criado_em")]
        public DateTime CriadoEm { get; set; }

        [Column("atualizado_em")]
        public DateTime? AtualizadoEm { get; set; }

        public virtual ICollection<CampingFoto> Fotos { get; set; } = [];

        public virtual ICollection<CampingRecurso> Recursos { get; set; } = [];

        public virtual ICollection<Trilha> Trilhas { get; set; } = [];

        public virtual ICollection<CampingAvaliacao> Avaliacoes { get; set; } = [];
    }
}
