using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.shared.Models.Requests
{
    public class CheckinRequest
    {
        public Guid UsuarioId { get; set; }

        public long CampingId { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}
