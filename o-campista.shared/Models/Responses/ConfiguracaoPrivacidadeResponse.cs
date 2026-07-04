namespace o_campista.shared.Models.Responses;

public class ConfiguracaoPrivacidadeResponse
{
    public bool PerfilPublico { get; set; }
    public bool CheckinsPublicos { get; set; }
    public bool ConquistasPublicas { get; set; }
    public bool NivelPublico { get; set; }
    public bool VisivelNoMapa { get; set; }
}
