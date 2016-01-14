using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.BeefBot.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<BeefEntry> BeefEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = SettingsAccessor.GetSetting<string>("DBConnectionString");
            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BeefEntry>().Ignore(x => x.ShortExplanation);
            modelBuilder.Entity<BeefEntry>().Ignore(x => x.HasExpired);
        }
    }
}
