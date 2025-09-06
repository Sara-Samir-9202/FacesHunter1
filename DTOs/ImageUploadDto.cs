// DTOs/ImageUploadDto.cs
using Microsoft.AspNetCore.Http;

namespace FacesHunter.DTOs
{
    public class ImageUploadDto
    {
        public IFormFile Image { get; set; } = default!;
    }
}

