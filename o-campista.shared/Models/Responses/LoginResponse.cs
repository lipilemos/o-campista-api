namespace o_campista.shared.Models.Responses;

public class LoginResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FotoPerfil { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public int Xp { get; set; }
    public int XpProximoNivel { get; set; }
    public int TotalCheckins { get; set; }
    public int TotalCampingsVisitados { get; set; }
    public int TotalTrilhasConcluidas { get; set; }
    public IEnumerable<ConquistaResponse> Conquistas { get; set; } = [];
    public IEnumerable<PresenteResponse> Presentes { get; set; } = [];
}
