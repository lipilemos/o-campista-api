using Microsoft.EntityFrameworkCore;
using o_campista.api.Context;
using o_campista.entities.Entities;
using o_campista.repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace o_campista.repository.imp.Repositories
{
    public class CampingRepository : ICampingRepository
    {
        private readonly CampistaDbContext _context;

        public CampingRepository(CampistaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Camping>> ObterCampingsMapaAsync(string? busca = null, string? tipo = null, string[]? recursos = null)
        {
            var query = _context.Campings
                .AsNoTracking()
                .Include(x => x.Fotos)
                .Include(x => x.Recursos)
                    .ThenInclude(x => x.Recurso)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = busca.ToLower();
                query = query.Where(c =>
                    c.Nome.ToLower().Contains(termo) ||
                    (c.Cidade != null && c.Cidade.ToLower().Contains(termo)) ||
                    (c.Estado != null && c.Estado.ToLower().Contains(termo)));
            }

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                query = query.Where(c => c.Tipo == tipo);
            }

            if (recursos is { Length: > 0 })
            {
                foreach (var recurso in recursos)
                {
                    var r = recurso;
                    query = query.Where(c =>
                        c.Recursos.Any(cr => cr.Recurso.Nome == r && cr.Disponivel));
                }
            }

            return await query.ToListAsync();
        }
        public async Task<Camping?> ObterPorIdAsync(long id)
        {
            return await _context.Campings
                .Include(x => x.Fotos)
                .Include(x => x.Recursos)
                    .ThenInclude(x => x.Recurso)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AtualizarAsync(Camping camping)
        {
            camping.AtualizadoEm = DateTime.UtcNow;
            _context.Update(camping);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarMediaAvaliacaoAsync(long campingId, decimal mediaAvaliacao)
        {
            await _context.Set<Camping>()
                .Where(c => c.Id == campingId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.AvaliacaoMedia, mediaAvaliacao)
                    .SetProperty(c => c.AtualizadoEm, DateTime.UtcNow));
        }

    }
}
