using Microsoft.AspNetCore.Http;

namespace o_campista.shared.Models.Requests;

public class PostViagemRequest
{
    public string Texto { get; set; } = string.Empty;
    public IFormFile? Foto { get; set; }
    public long? CampingId { get; set; }
    public long? TrilhaId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
