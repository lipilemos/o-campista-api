using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.imp.Repositories
{
    public class TrilhaRepository : ITrilhaRepository
    {
        private readonly CampistaDbContext _context;

        public TrilhaRepository(CampistaDbContext context)
        {
            _context = context;
        }
        public async Task<int> ContarConcluidasPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.UsuarioTrilhas
                .CountAsync(x =>
                    x.UsuarioId == usuarioId &&
                    x.Concluida);
        }
    }
}
