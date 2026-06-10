using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.imp.Repositories
{
    public class UsuarioConquistaRepository : IUsuarioConquistaRepository
    {
        private readonly CampistaDbContext _context;

        public UsuarioConquistaRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteAsync(Guid usuarioId,long conquistaId)
        {
            return await _context.UsuarioConquistas
                .AnyAsync(x =>
                    x.UsuarioId == usuarioId &&
                    x.ConquistaId == conquistaId);
        }
        public async Task CriarAsync(UsuarioConquista conquista)
        {
            await _context.UsuarioConquistas
                .AddAsync(conquista);

            await _context.SaveChangesAsync();
        }
    }
}
