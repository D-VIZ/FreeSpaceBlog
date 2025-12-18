namespace FreeSpace.Models
{
    public class FollowedPost
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public int PostId { get; set; }
        public Post? Post { get; set; }

        public DateTime LastRead { get; set; }
    }
}
