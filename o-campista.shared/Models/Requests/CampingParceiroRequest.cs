using System.ComponentModel.DataAnnotations;

namespace o_campista.shared.Models.Requests;

public class CampingParceiroRequest
{
    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    /// <summary>"camping" ou "pesca".</summary>
    [Required, MaxLength(50)]
    public string Tipo { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    [MaxLength(500)]
    public string? Endereco { get; set; }

    [Required, MaxLength(100)]
    public string Cidade { get; set; } = string.Empty;

    [Required, StringLength(2, MinimumLength = 2)]
    public string Estado { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Telefone { get; set; }

    [Required, Range(-90, 90)]
    public decimal Latitude { get; set; }

    [Required, Range(-180, 180)]
    public decimal Longitude { get; set; }

    public List<long> RecursosIds { get; set; } = [];
}
