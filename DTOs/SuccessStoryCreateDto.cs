// DTOs/SuccessStoryCreateDto.cs

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FacesHunter.DTOs
{
    public class SuccessStoryCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Video file is required")]
        public IFormFile Video { get; set; } = null!;

        public DateTime? DatePosted { get; set; }

        
    }
}




