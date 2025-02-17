using Microsoft.EntityFrameworkCore;

namespace Peactica_API.Models
{
    public class Practica1Context : DbContext
    {
        public Practica1Context(DbContextOptions<Practica1Context> options)
            : base(options)
        {
        }
        public DbSet<Restaurantes> Restaurantes { get; set; }
    }
}
    