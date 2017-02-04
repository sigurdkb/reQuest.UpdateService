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
            // optionsBuilder.UseSqlite(@"Filename=/Users/sigurdkb/repos/reQuest/reQuest.Backend/bin/Debug/netcoreapp1.1/db/reQuest.db");
        }

        // public DbSet<Player> Players { get; set; }
        // public DbSet<Topic> Topics { get; set; }
        public DbSet<Quest> Quests { get; set; }

    }
}
