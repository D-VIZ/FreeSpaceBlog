using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FreeSpace.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 4)]
        [DisplayName("Título")]
        public string Title { get; set; }

        [DisplayName("Descrição")]
        [StringLength(300)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Postado em:")]
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        [Display(Name = "Mídia")]
        public IFormFile? Media { get; set; }

        public string? MediaPath { get; set; }

        public int Likes { get; set; } = 0;
    }
}
