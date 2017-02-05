using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
