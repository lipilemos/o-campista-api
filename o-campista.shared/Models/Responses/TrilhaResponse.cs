namespace o_campista.shared.Models.Responses
{
    public class TrilhaResponse
    {
        public long Id { get; set; }
        public long? CampingId { get; set; }
        public string? CriadorNome { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal DistanciaKm { get; set; }
        public string? Dificuldade { get; set; }
        public double AvaliacaoMedia { get; set; }
        public int CheckinsRecentes { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime CriadoEm { get; set; }
        public List<TrilhaPontoResponse> Pontos { get; set; } = [];
        public bool ConcluidaPeloUsuario { get; set; }
    }

    public class TrilhaPontoResponse
    {
        public long Id { get; set; }
        public int Ordem { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
