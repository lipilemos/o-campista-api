namespace o_campista.api.Models.Responses
{
    public class LoginResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public int Nivel { get; set; }
        public int Xp { get; set; }        
    }
}
