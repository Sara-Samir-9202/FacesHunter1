//Data/SeedPersons.cs
using FacesHunter.Models;
using FacesHunter.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace FacesHunter.Data
{
    public static class SeedPersons
    {
        public static async Task SeedAsync(AppDbContext context, AiService aiService, IWebHostEnvironment env)
        {
            if (await context.Persons.AnyAsync())
                return;

            var seedFolder = Path.Combine(env.WebRootPath, "seed_faces");

            if (!Directory.Exists(seedFolder))
                return;

            var images = Directory.GetFiles(seedFolder);

            foreach (var imgPath in images)
            {
                var fileName = Path.GetFileName(imgPath);
                await using var stream = new FileStream(imgPath, FileMode.Open, FileAccess.Read);

                var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg"
                };

                var embedding = await aiService.GenerateFaceEmbeddingAsync(formFile);
                var embeddingJson = JsonSerializer.Serialize(embedding);

                var person = new Person
                {
                    FullName = Path.GetFileNameWithoutExtension(fileName),
                    Age = new Random().Next(10, 60),
                    Gender = Gender.Male,
                    LastSeenLocation = "Test City",
                    DateMissing = DateTime.UtcNow.AddDays(-new Random().Next(1, 30)),
                    ImagePath = $"/seed_faces/{fileName}",
                    ReporterName = "Seeder",
                    ReporterPhone = "0123456789",
                    FaceEmbedding = embeddingJson
                };

                context.Persons.Add(person);
                await context.SaveChangesAsync();
            }
        }
    }
}


