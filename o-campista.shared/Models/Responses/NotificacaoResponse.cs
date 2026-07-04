namespace o_campista.shared.Models.Responses;

public class NotificacaoResponse
{
    public long Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public bool Lida { get; set; }
    public DateTime CriadoEm { get; set; }
    public string RemetenteId { get; set; } = string.Empty;
    public string RemetenteNome { get; set; } = string.Empty;
    public string? RemetenteFoto { get; set; }
    public long? PostId { get; set; }
    public string? PostTexto { get; set; }
    public string? ComentarioTexto { get; set; }
}
