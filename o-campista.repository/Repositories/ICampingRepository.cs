using o_campista.entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.Repositories
{
    public interface ICampingRepository
    {
        Task<IEnumerable<Camping>> ObterCampingsMapaAsync();
    }
}

