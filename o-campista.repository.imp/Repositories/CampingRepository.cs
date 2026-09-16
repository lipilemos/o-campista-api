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

            query = query.Where(c => c.Ativo);

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

        public async Task<Camping> CriarAsync(Camping camping)
        {
            camping.CriadoEm = DateTime.UtcNow;
            await _context.Campings.AddAsync(camping);
            await _context.SaveChangesAsync();
            return camping;
        }

        public Task<List<Camping>> ObterPorDonoAsync(Guid usuarioId)
        {
            return _context.Campings
                .AsNoTracking()
                .Where(c => c.DonoUsuarioId == usuarioId)
                .OrderByDescending(c => c.CriadoEm)
                .ToListAsync();
        }

        public async Task<List<(Camping Camping, double DistanciaMetros)>> ObterSemDonoNoRaioAsync(
            decimal latitude, decimal longitude, double raioMetros)
        {
            // Pré-filtro por bounding box (1 grau ≈ 111 km) para não trazer a tabela inteira;
            // a distância exata é calculada em memória com Haversine.
            var delta = (decimal)(raioMetros / 111_000d);
            var candidatos = await _context.Campings
                .AsNoTracking()
                .Where(c => c.Ativo
                    && c.DonoUsuarioId == null
                    && Camping.TiposComDono.Contains(c.Tipo)
                    && c.Latitude >= latitude - delta && c.Latitude <= latitude + delta
                    && c.Longitude >= longitude - delta && c.Longitude <= longitude + delta)
                .ToListAsync();

            return candidatos
                .Select(c => (Camping: c, DistanciaMetros: DistanciaMetros(
                    (double)latitude, (double)longitude, (double)c.Latitude, (double)c.Longitude)))
                .Where(x => x.DistanciaMetros <= raioMetros)
                .OrderBy(x => x.DistanciaMetros)
                .ToList();
        }

        public Task<int> ContarFavoritosAsync(long campingId)
        {
            return _context.UsuarioCampingFavoritos.CountAsync(f => f.CampingId == campingId);
        }

        public Task<int> ContarAvaliacoesAsync(long campingId)
        {
            return _context.CampingAvaliacoes.CountAsync(a => a.CampingId == campingId);
        }

        private static double DistanciaMetros(double lat1, double lng1, double lat2, double lng2)
        {
            const double raioTerra = 6_371_000d;
            var dLat = (lat2 - lat1) * Math.PI / 180d;
            var dLng = (lng2 - lng1) * Math.PI / 180d;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                    + Math.Cos(lat1 * Math.PI / 180d) * Math.Cos(lat2 * Math.PI / 180d)
                    * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return raioTerra * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
    }
}
