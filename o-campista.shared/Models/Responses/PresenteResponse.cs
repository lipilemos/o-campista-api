using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace o_campista.shared.Models.Responses;

public class PresenteResponse
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string CodigoResgate { get; set; } = string.Empty;
    public bool Utilizado { get; set; }
    public string FotoUrl { get; set; } = string.Empty;
    public double Latitude{ get; set; }
    public double Longitude{ get; set; }
    public Guid UsuarioCriadorId { get; set; }
    public bool? EstaDisponivel { get; set; }
    public DateTime? CriadoEm { get; set; } = DateTime.UtcNow;
}
