using Microsoft.AspNetCore.Identity;
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

        [Required(ErrorMessage = "O campo título deve estar preenchido")]
        [StringLength(30, MinimumLength = 4, ErrorMessage = "O campo deve ter no mínimo 4 caracteres e no máximo 30")]
        [DisplayName("Título")]
        public string Title { get; set; }

        [DisplayName("Descrição")]
        [StringLength(300)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        [Display(Name = "Postado em:")]
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        [Display(Name = "Mídia")]
        public IFormFile? Media { get; set; }

        [Required]
        public string Tag { get; set; } = "Geral";

        public string? MediaPath { get; set; }

        public string? TagPath { get; set; }

        public int Likes { get; set; } = 0;

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<Like> LikesList { get; set; } = new List<Like>();
    }
}
