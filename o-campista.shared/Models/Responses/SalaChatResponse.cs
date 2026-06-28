namespace o_campista.shared.Models.Responses
{
    public class SalaChatResponse
    {
        public long Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public long? CampingId { get; set; }
        public string? FotoCapa { get; set; }
        public string? CodigoConvite { get; set; }
        public DateTime DataCriacao { get; set; }
        public UltimaMensagemResponse? UltimaMensagem { get; set; }
        public int TotalNaoLidas { get; set; }
        public bool PodeEnviar { get; set; }
    }

    public class UltimaMensagemResponse
    {
        public string Texto { get; set; } = null!;
        public string NomeUsuario { get; set; } = null!;
        public DateTime DataEnvio { get; set; }
    }
}
