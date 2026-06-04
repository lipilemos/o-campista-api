namespace o_campista.api.Models.Responses
{
    public class CampingMapaResponse
    {
        public long Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public double Avaliacao { get; set; }
        public string FotoPrincipal { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public List<RecursoCampingResponse> Recursos { get; set; } = [];
    }
}
