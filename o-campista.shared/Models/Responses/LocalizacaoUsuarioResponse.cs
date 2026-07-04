namespace o_campista.shared.Models.Responses;

public class LocalizacaoUsuarioResponse
{
    public string UsuarioId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
}
