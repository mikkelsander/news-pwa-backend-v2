using Microsoft.EntityFrameworkCore;
using PWANews.Models.DomainModels;

namespace PWANews.Data
{
    public class PWANewsDbContext : DbContext
    {
        public PWANewsDbContext(DbContextOptions<PWANewsDbContext> options) 
            : base(options)
        {  }

        public DbSet<User> Users { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(User => User.Username)
                .IsUnique();

            modelBuilder.Entity<Subscription>()
                .HasKey(sub => new { sub.UserId, sub.PublisherId });

            modelBuilder.Entity<Subscription>()
                .HasOne(sub => sub.User)
                .WithMany(user => user.Subscriptions)
                .HasForeignKey(sub => sub.UserId);

            modelBuilder.Entity<Subscription>()
                .HasOne(sub => sub.Publisher)
                .WithMany(publisher => publisher.Subscriptions)
                .HasForeignKey(sub => sub.PublisherId);
        }
    }
}
