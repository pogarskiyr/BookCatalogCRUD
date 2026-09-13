using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCatalog.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя автора обязательно")]
        [Display(Name = "Полное имя")]
        public string FullName { get; set; }

        [Display(Name = "Год рождения")]
        public int? BirthYear { get; set; }

        
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}