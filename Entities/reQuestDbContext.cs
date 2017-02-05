using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace reQuest.UpdateService.Entities
{
    public class reQuestDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Program.Configuration.GetConnectionString("reQuestDb"));
        }

        public DbSet<Quest> Quests { get; set; }

    }
}
