using System.ComponentModel.DataAnnotations;

namespace FreeSpace.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo não pode estar vazio")]
        [StringLength(300)]
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }

        public int PostId { get; set; }
        public Post? Post { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
