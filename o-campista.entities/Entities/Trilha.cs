using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace o_campista.entities.Entities
{
    [Table("tb_trilha")]
    public class Trilha
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("camping_id")]
        public long? CampingId { get; set; }

        [Column("criador_id")]
        public Guid? CriadorId { get; set; }

        [Column("criador_nome")]
        [MaxLength(200)]
        public string? CriadorNome { get; set; }

        [Column("nome")]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        public string? Descricao { get; set; }

        [Column("distancia_km")]
        public decimal DistanciaKm { get; set; }

        [Column("dificuldade")]
        [MaxLength(50)]
        public string? Dificuldade { get; set; }

        [Column("avaliacao_media")]
        public double AvaliacaoMedia { get; set; }

        [Column("latitude")]
        public decimal Latitude { get; set; }

        [Column("longitude")]
        public decimal Longitude { get; set; }

        [Column("criado_em")]
        public DateTime CriadoEm { get; set; }

        [ForeignKey(nameof(CampingId))]
        public virtual Camping? Camping { get; set; }

        public virtual ICollection<TrilhaPonto> Pontos { get; set; } = [];
    }
}
