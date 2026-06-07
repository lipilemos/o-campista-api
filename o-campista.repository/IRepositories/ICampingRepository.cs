using o_campista.entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.IRepositories
{
    public interface ICampingRepository
    {
        Task<IEnumerable<Camping>> ObterCampingsMapaAsync();
        Task<Camping?> ObterPorIdAsync(long id);
    }
}

