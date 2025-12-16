using Microsoft.Extensions.FileProviders;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreeSpace.Models
{
    public class Tutorials
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo descrição deve estar preenchido")]
        [StringLength(1000, ErrorMessage = "O campo não pode ultrapassar 1000 caracteres")]
        [DisplayName("Descrição")]
        public string Description { get; set; }

        [DisplayName("Embed URL")]
        public string? url { get; set; } = null;

        [DisplayName("Software")]
        public string? Plataform { get; set; } = "Não especificado";

        [NotMapped]
        public IFormFile? Media { get; set; }

        public string? FilePath { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
