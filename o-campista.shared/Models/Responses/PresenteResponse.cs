namespace o_campista.shared.Models.Responses;

public class PresenteResponse
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string CodigoResgate { get; set; } = string.Empty;
    public bool Utilizado { get; set; }
}
