using Microsoft.EntityFrameworkCore;
using o_campista.api.Entidades;

namespace o_campista.api.Context
{
    public class CampistaDbContext : DbContext
    {
        public CampistaDbContext(
            DbContextOptions<CampistaDbContext> options
        ) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
