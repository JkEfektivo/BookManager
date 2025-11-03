using BookManager.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManager.Data
{
    public class BookManagerContext : DbContext
    {
        public BookManagerContext(DbContextOptions<BookManagerContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Walidacja roku wydania
            modelBuilder.Entity<Book>()
                .ToTable(t => t.HasCheckConstraint("CK_Book_YearPublished",
                    "\"YearPublished\" >= 1000 AND \"YearPublished\" <= CAST(strftime('%Y', 'now') AS INTEGER)"));

            // Index dla sortowania po DateAdded
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.DateAdded)
                .IsDescending();
        }
    }
}
