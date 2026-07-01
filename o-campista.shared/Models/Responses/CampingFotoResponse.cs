namespace o_campista.shared.Models.Responses
{
    public class CampingFotoResponse
    {
        public long Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool Principal { get; set; }
        public int Ordem { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
