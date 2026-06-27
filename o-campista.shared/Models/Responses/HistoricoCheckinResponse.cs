namespace o_campista.shared.Models.Responses
{
    public class HistoricoCheckinResponse
    {
        public long Id { get; set; }
        public Guid UsuarioId { get; set; }
        public long CampingId { get; set; }
        public CampingInfoResponse Camping { get; set; } = null!;
        public DateTime DataCriacao { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class CampingInfoResponse
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string FotoPrincipal{ get; set; } = string.Empty;

    }
}

