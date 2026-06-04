using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.shared.Models.Responses
{
    public class CheckinResponse
    {
        public bool Sucesso { get; set; }

        public int XpGanho { get; set; }

        public int NivelAtual { get; set; }

        public string Mensagem { get; set; } = string.Empty;
    }
}
