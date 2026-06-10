using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.imp.Repositories
{
    public class UsuarioPresenteRepository:IUsuarioPresenteRepository
    {
        private readonly CampistaDbContext _context;

        public UsuarioPresenteRepository(CampistaDbContext context)
        {
            _context = context;
        }
        public async Task<int> ContarResgatadosPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.UsuarioPresentes
                .CountAsync(x =>
                    x.UsuarioId == usuarioId &&
                    x.Utilizado);
        }
        public async Task CriarAsync(UsuarioPresente usuarioPresente)
        {
            await _context.UsuarioPresentes.AddAsync(usuarioPresente);

            await _context.SaveChangesAsync();
        }
    }
}
