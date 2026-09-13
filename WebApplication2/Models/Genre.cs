using System.ComponentModel.DataAnnotations;

namespace BookCatalog.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название жанра обязательно")]
        [Display(Name = "Жанр")]
        public string Name { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}