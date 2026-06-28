using System.ComponentModel.DataAnnotations;

namespace o_campista.shared.Models.Requests
{
    public class CriarGrupoRequest
    {
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = null!;
    }
}
