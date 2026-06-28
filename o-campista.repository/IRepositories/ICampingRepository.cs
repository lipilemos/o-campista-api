using o_campista.entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.IRepositories
{
    public interface ICampingRepository
    {
        Task<IEnumerable<Camping>> ObterCampingsMapaAsync(string? busca = null, string? tipo = null, string[]? recursos = null);
        Task<Camping?> ObterPorIdAsync(long id);
        Task AtualizarAsync(Camping camping);
        Task AtualizarMediaAvaliacaoAsync(long campingId, decimal mediaAvaliacao);
    }
}

