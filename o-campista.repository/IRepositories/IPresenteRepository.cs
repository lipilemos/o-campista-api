using o_campista.entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.IRepositories
{
    public interface IPresenteRepository
    {
        Task CriarAsync(Presente presente);
        Task<IEnumerable<Presente>> ObterPresentesPorRaioAsync(double latitude, double longitude);
    }
}
