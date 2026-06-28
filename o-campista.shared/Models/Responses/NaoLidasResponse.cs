namespace o_campista.shared.Models.Responses
{
    public class NaoLidasResponse
    {
        public int Total { get; set; }
        public Dictionary<long, int> PorSala { get; set; } = new();
    }
}
