namespace o_campista.shared.Models.Responses;

public class CampingPainelResponse
{
    public long CampingId { get; set; }
    public int Checkins30Dias { get; set; }
    public int CheckinsTotal { get; set; }
    public int VisitantesUnicos { get; set; }
    public decimal AvaliacaoMedia { get; set; }
    public int TotalAvaliacoes { get; set; }
    public int TotalFavoritos { get; set; }
    public StatusOcupacaoResponse? StatusOcupacao { get; set; }
    public List<CheckinsDiaResponse> CheckinsPorDia { get; set; } = [];
}

public class CheckinsDiaResponse
{
    public DateOnly Data { get; set; }
    public int Quantidade { get; set; }
}
