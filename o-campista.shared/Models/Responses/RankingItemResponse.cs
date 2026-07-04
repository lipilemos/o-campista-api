namespace o_campista.shared.Models.Responses;

public class RankingItemResponse
{
    public int Posicao { get; set; }
    public Guid UsuarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? FotoPerfil { get; set; }
    public int Nivel { get; set; }
    public int Xp { get; set; }
    public int TotalCheckins { get; set; }
    public bool EstouSeguindo { get; set; }
}
