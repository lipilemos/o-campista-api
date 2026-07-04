namespace o_campista.shared.Models.Responses;

public class CampingRankingResponse
{
    public int Posicao { get; set; }
    public long CampingId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? FotoPrincipal { get; set; }
    public decimal AvaliacaoMedia { get; set; }
    public int TotalCheckins { get; set; }
}
