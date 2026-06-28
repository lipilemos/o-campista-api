using System.ComponentModel.DataAnnotations;

namespace o_campista.shared.Models.Requests
{
    public class EntrarGrupoRequest
    {
        [Required]
        public string CodigoConvite { get; set; } = null!;
    }
}
