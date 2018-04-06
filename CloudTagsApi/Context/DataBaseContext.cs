using CloudTagsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudTagsApi.Context
{
    public class DataBaseContext : DbContext
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options)
        {

        }
        public DbSet<WordEntities> Words { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
