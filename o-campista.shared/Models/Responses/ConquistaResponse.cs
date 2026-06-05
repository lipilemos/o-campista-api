
namespace o_campista.shared.Models.Responses;

public class ConquistaResponse
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;
    public DateTime DataConquista { get; set; }
}
