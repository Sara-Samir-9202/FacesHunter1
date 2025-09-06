//Services/FileService.cs
using Microsoft.AspNetCore.Http;

namespace FacesHunter.Services
{
    public class FileService
    {
        public async Task<string> SaveImageAsync(IFormFile file, string folderName)
        {
            var uploadsFolder = Path.Combine("wwwroot", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folderName}/{fileName}";
        }
    }
}



