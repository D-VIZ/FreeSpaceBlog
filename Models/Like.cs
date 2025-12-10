using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreeSpace.Models
{
    [Table("Like")]
    public class Like
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post? Post { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
