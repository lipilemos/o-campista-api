namespace o_campista.shared.Models.Responses;

public class AcessoAchadosResponse
{
    /// <summary>True se o usuário já fez check-in neste camping alguma vez.</summary>
    public bool PodeVer { get; set; }

    /// <summary>True se o usuário fez check-in neste camping nas últimas 24h.</summary>
    public bool PodePublicar { get; set; }
}
