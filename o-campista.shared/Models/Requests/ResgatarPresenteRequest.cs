using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.shared.Models.Requests
{
    public class ResgatarPresenteRequest
    {
        public Guid UsuarioId { get; set; }
        public long PresenteId { get; set; }
    }
}
