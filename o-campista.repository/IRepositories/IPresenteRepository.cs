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
        Task<int> ContarCriadosPorUsuarioAsync(Guid usuarioId);
        Task<Presente?> ObterPorIdAsync(long id);
        Task AtualizarAsync(Presente presente);
        Task MarcarComoResgatadoAsync(Presente presenteId);
        Task DeletarAsync(Presente presente);
    }
}
