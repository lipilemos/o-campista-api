using o_campista.entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.IRepositories
{
    public interface ICheckinRepository
    {
        Task CriarAsync(Checkin checkin);
        Task<bool> JaExisteHojeAsync(Guid usuarioId, long campingId);
        Task<int> ObterQuantidadeCheckinsUsuarioAsync(Guid usuarioId);
        Task<int> ContarPorUsuarioAsync(Guid usuarioId);
        Task<List<Checkin>> ObtenerHistoricoAsync(Guid usuarioId);
        Task<bool> TemCheckinNasUltimas24hAsync(Guid usuarioId, long campingId);
        Task<int> ContarCheckinsUltimas24hAsync(long campingId);
        Task<bool> JaExisteTrilhaHojeAsync(Guid usuarioId, long trilhaId);
        Task<int> ContarCheckinsUltimas24hTrilhaAsync(long trilhaId);
        Task<int> ContarCheckinsTrilhaPorUsuarioAsync(Guid usuarioId);
    }
}
