using System.ComponentModel.DataAnnotations;

namespace BookManager.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Tytuł musi mieć między 1 a 255 znaków")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Autor jest wymagany")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "Autor musi mieć między 1 a 255 znaków")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rok wydania jest wymagany")]
        [Range(1000, 9999, ErrorMessage = "Rok wydania musi być między 1000 a rokiem bieżącym")]
        public int YearPublished { get; set; }

        [Required]
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
