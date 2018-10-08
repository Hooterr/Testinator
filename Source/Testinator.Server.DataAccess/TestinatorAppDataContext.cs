using Microsoft.EntityFrameworkCore;
using Testinator.Server.DataAccess.Entities;

namespace Testinator.Server.DataAccess
{
    public class TestinatorAppDbContext : DbContext
    {
        public TestinatorAppDbContext(DbContextOptions<TestinatorAppDbContext> options) : base(options) { }

        public DbSet<Setting> Settings { get; set; }
    }
}
