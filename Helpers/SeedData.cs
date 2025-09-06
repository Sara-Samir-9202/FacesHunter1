//Helpers/SeedData.cs
using FacesHunter.Data;
using FacesHunter.Models;
using FacesHunter.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace FacesHunter.Helpers
{
    public static class SeedData
    {
        public static async Task SeedPersonsAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            var aiService = scope.ServiceProvider.GetRequiredService<AiService>();

            string[] names = { "Ahmed Ali", "Sara Youssef", "Mohamed Gamal", "Layla Adel", "Omar Tarek" };
            string[] locations = { "Cairo", "Alexandria", "Giza", "Mansoura", "Aswan" };
            string[] imagePaths = Directory.GetFiles(Path.Combine(env.WebRootPath, "seed_images"));

            for (int i = 0; i < 5; i++)
            {
                var filePath = imagePaths[i];
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(filePath)}";
                var destPath = Path.Combine(env.WebRootPath, "images", fileName);
                File.Copy(filePath, destPath, overwrite: true);

                // 🧠 ArcFace Embedding
                using var stream = File.OpenRead(filePath);
                var formFile = new FormFile(stream, 0, stream.Length, null!, Path.GetFileName(filePath));
                var embedding = await aiService.GenerateFaceEmbeddingAsync(formFile);
                var embeddingJson = JsonSerializer.Serialize(embedding);

                var person = new Person
                {
                    FullName = names[i],
                    Age = 20 + i * 5,
                    Gender = i % 2 == 0 ? Gender.Male : Gender.Female,
                    LastSeenLocation = locations[i],
                    DateMissing = DateTime.UtcNow.AddDays(-i * 2),
                    ImagePath = $"/images/{fileName}",
                    ReporterName = "System Seeder",
                    ReporterPhone = "0100000000",
                 
                    FaceEmbedding = embeddingJson
                };

                context.Persons.Add(person);
            }

            await context.SaveChangesAsync();
        }
    }
}



