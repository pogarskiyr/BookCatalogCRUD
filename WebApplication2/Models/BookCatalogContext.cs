using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Models
{
    public class BookCatalogContext : DbContext
    {
        public BookCatalogContext(DbContextOptions<BookCatalogContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связей (один ко многим)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // Не даём удалить автора, если есть книги

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Seed-данные (начальное наполнение) ---
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FullName = "Лев Толстой", BirthYear = 1828 },
                new Author { Id = 2, FullName = "Фёдор Достоевский", BirthYear = 1821 },
                new Author { Id = 3, FullName = "Агата Кристи", BirthYear = 1890 }
            );

            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Роман" },
                new Genre { Id = 2, Name = "Детектив" },
                new Genre { Id = 3, Name = "Фантастика" }
            );

            // Книги (добавим пару для примера)
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Война и мир", Year = 1869, Price = 500, AuthorId = 1, GenreId = 1 },
                new Book { Id = 2, Title = "Преступление и наказание", Year = 1866, Price = 450, AuthorId = 2, GenreId = 1 },
                new Book { Id = 3, Title = "Убийство в Восточном экспрессе", Year = 1934, Price = 600, AuthorId = 3, GenreId = 2 }
            );
        }
    }
}