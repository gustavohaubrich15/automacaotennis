using AutomationTennis.Domain;
using Microsoft.EntityFrameworkCore;

namespace AutomationTennis.Context
{
    public class AutomationTennisContext : DbContext
    {
        public DbSet<TournamentWTA> TournamentWTA { get; set; }
        public DbSet<TournamentGroupWTA> TournamentGroupWTA { get; set; }

        public AutomationTennisContext(DbContextOptions<AutomationTennisContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TournamentWTA>()
                .HasOne(t => t.TournamentGroupWTA)
                .WithOne(g => g.TournamentWTA)
                .HasForeignKey<TournamentGroupWTA>(g => g.IdTournamentWTA);

            base.OnModelCreating(modelBuilder);
        }

    }
}
