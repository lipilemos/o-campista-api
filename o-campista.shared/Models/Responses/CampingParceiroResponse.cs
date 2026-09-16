namespace o_campista.shared.Models.Responses;

public class CampingParceiroResponse
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool Ativo { get; set; }
    public string DonoStatus { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
