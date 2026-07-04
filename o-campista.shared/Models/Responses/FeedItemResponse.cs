namespace o_campista.shared.Models.Responses;

public class FeedItemResponse
{
    public long Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string? UsuarioFoto { get; set; }
    public DateTime CriadoEm { get; set; }

    // tipo = checkin
    public long? CampingId { get; set; }
    public string? CampingNome { get; set; }
    public string? CampingFoto { get; set; }
    public string? OcupacaoReportada { get; set; }

    // tipo = conquista
    public string? ConquistaNome { get; set; }
    public string? ConquistaDescricao { get; set; }
    public string? ConquistaIcone { get; set; }

    // tipo = avaliacao
    public int? Nota { get; set; }
    public string? Comentario { get; set; }
    public string? AvaliacaoFotoUrl { get; set; }
    public string? LocalNome { get; set; }

    // tipo = post
    public long? PostId { get; set; }
    public string? PostTexto { get; set; }
    public string? PostFotoUrl { get; set; }
    public int? TotalCurtidas { get; set; }
    public int? TotalComentarios { get; set; }
    public bool? Curtiu { get; set; }

    // tipo = trilha_concluida
    public string? TrilhaNome { get; set; }
    public decimal? DistanciaKm { get; set; }
    public string? Dificuldade { get; set; }
}
