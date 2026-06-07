
namespace o_campista.shared.Models.Requests;
using Microsoft.AspNetCore.Http;

public class PresenteCreateRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public IFormFile Foto { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string? CodigoResgate { get; set; }
    public Guid UsuarioCriadorId { get; set; }
}
