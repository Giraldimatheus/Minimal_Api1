using Microsoft.EntityFrameworkCore;
using Minimal_Api1.Models;

namespace Minimal_Api1.Data
{
    public class ChampionsContext : DbContext
    {
        public ChampionsContext(DbContextOptions<ChampionsContext> options) : base (options) { }

        public DbSet<Champions> Champions { get; set; }

    }
}
