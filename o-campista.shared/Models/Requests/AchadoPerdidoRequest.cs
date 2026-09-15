using Microsoft.AspNetCore.Http;

namespace o_campista.shared.Models.Requests;

public class AchadoPerdidoRequest
{
    /// <summary>"achado" ou "perdido".</summary>
    public string Tipo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string? LocalGuarda { get; set; }
    public IFormFile? Foto { get; set; }
}
