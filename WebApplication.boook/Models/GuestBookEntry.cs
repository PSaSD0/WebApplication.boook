using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.boook.Models
{
    public class GuestBookEntry
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Автор обязателен")]
        [Display(Name = "Автор")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "Текст обязателен")]
        [Display(Name = "Текст отзыва")]
        public string Text { get; set; } = string.Empty;

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}