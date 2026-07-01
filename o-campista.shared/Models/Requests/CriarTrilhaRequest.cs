namespace o_campista.shared.Models.Requests
{
    public class CriarTrilhaRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? Dificuldade { get; set; }
        public Guid CriadorId { get; set; }
        public string CriadorNome { get; set; } = string.Empty;
        public List<TrilhaPontoRequest> Pontos { get; set; } = [];
    }

    public class TrilhaPontoRequest
    {
        public int Ordem { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
