using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.business.imp.Dictionaries
{
    public static class NivelXpDictionary
    {
        public static readonly Dictionary<int, int> XpPorNivel = new()
    {
        { 1, 800 },
        { 2, 2000 },
        { 3, 3500 },
        { 4, 5500 },
        { 5, 8000 },
        { 6, 11000 },
        { 7, 15000 },
        { 8, 20000 },
        { 9, 26000 },
        { 10, 33000 },
        { 11, 41000 },
        { 12, 50000 },
        { 13, 60000 },
        { 14, 71000 },
        { 15, 83000 },
        { 16, 96000 },
        { 17, 110000 },
        { 18, 125000 },
        { 19, 141000 }
    };

        public static int ObterXpProximoNivel(int nivel)
        {
            return XpPorNivel.TryGetValue(nivel, out var xp)
                ? xp
                : int.MaxValue;
        }
    }
}
