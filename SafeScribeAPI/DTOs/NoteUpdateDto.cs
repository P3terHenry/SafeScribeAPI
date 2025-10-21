using System.ComponentModel.DataAnnotations;

namespace SafeScribeAPI.DTOs
{
    public class NoteUpdateDto
    {
        [Required]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;
    }
}
