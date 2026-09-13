using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;

namespace BookCatalog.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название книги обязательно")]
        [Display(Name = "Название")]
        public string Title { get; set; }

        [Display(Name = "Год издания")]
        public int? Year { get; set; }

        [Display(Name = "Цена")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        // Внешние ключи
        public int AuthorId { get; set; }
        public int GenreId { get; set; }

        // Навигационные свойства
        [ForeignKey("AuthorId")]
        public Author Author { get; set; }

        [ForeignKey("GenreId")]
        public Genre Genre { get; set; }
    }
}