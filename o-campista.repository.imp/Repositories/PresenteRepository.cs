using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace o_campista.repository.imp.Repositories
{
    public class PresenteRepository : IPresenteRepository
    {
        private readonly CampistaDbContext _context;

        public PresenteRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task CriarAsync(Presente presente)
        {
            await _context.Set<Presente>().AddAsync(presente);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Presente>> ObterPresentesPorRaioAsync(double latitude, double longitude)
        {
            var presents = await _context.BuscarPresentesProximos(latitude, longitude, 10000)
                .AsNoTracking()
                .ToListAsync();
            return presents;
        }
        public async Task<int> ContarCriadosPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Presentes
                .CountAsync(x =>
                    x.UsuarioCriadorId == usuarioId);
        }
        public async Task<Presente?> ObterPorIdAsync(long id)
        {
            return await _context.Presentes
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AtualizarAsync(Presente presente)
        {
            _context.Presentes.Update(presente);

            await _context.SaveChangesAsync();
        }

        public async Task MarcarComoResgatadoAsync(Presente presente)
        {
            presente.EstaDisponivel = false;

            _context.Entry(presente)
                .Property(x => x.EstaDisponivel)
                .IsModified = true;

            await _context.SaveChangesAsync();
        }
    }
}
