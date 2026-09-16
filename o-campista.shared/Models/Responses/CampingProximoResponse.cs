namespace o_campista.shared.Models.Responses;

public class CampingProximoResponse
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public double DistanciaMetros { get; set; }
}
