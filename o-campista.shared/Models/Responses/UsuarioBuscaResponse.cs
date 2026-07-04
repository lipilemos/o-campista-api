namespace o_campista.shared.Models.Responses;

public class UsuarioBuscaResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string FotoPerfil { get; set; } = string.Empty;
    public int Nivel { get; set; }
}
