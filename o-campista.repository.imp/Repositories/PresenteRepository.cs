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
    }
}
