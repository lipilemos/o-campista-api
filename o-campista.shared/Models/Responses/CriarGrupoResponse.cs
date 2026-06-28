namespace o_campista.shared.Models.Responses
{
    public class CriarGrupoResponse
    {
        public SalaChatResponse Sala { get; set; } = null!;
        public string CodigoConvite { get; set; } = null!;
    }
}
