
// Services/AiService.cs
using FacesHunter.Data;
using FacesHunter.Helpers;
using FacesHunter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FacesHunter.Services
{
    public class AiService
    {
        private readonly HttpClient _http;
        private readonly AppDbContext _db;

        public AiService(HttpClient http, AppDbContext db)
        {
            _http = http;
            _db = db;
        }

        // ✅ ArcFace - استخراج Embedding فقط (للتخزين)
        public async Task<List<double>?> GenerateFaceEmbeddingAsync(IFormFile image)
        {
            using var content = new MultipartFormDataContent();
            using var stream = image.OpenReadStream();
            content.Add(new StreamContent(stream), "image", image.FileName);

            var response = await _http.PostAsync(AiEndpoints.ArcFaceEmbedding, content);


            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<double>>(json);
        }

        // ✅ ArcFace - مقارنة الوجه
        public async Task<FaceMatchVectorResult?> CompareFaceAsync(IFormFile image)
        {
            using var content = new MultipartFormDataContent();

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var fileContent = new ByteArrayContent(ms.ToArray());
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(image.ContentType);
            content.Add(fileContent, "file", image.FileName);

            var response = await _http.PostAsync(AiEndpoints.ArcFaceEmbedding, content);


            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FaceMatchVectorResult>(json);
        }

        // ✅ SAM: توليد صورة بعمر أكبر
        public async Task<byte[]?> GenerateOlderFaceAsync(IFormFile image, int targetAge)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StreamContent(image.OpenReadStream()), "image", image.FileName);
            content.Add(new StringContent(targetAge.ToString()), "target_age");

            var response = await _http.PostAsync(AiEndpoints.ArcFaceEmbedding, content);


            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsByteArrayAsync();
        }

        // ✅ OCR: استخراج اسم من صورة بطاقة
        public async Task<string?> ExtractNameFromIdAsync(IFormFile idImage)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(idImage.OpenReadStream()), "image", idImage.FileName);

            var response = await _http.PostAsync(AiEndpoints.ArcFaceEmbedding, content);


            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }

        // ✅ البحث عن أقرب شخص بناءً على Embedding
        public async Task<Person?> FindBestMatchAsync(IFormFile image)
        {
            var faceResult = await CompareFaceAsync(image);
            if (faceResult?.Vectors == null || !faceResult.Vectors.Any())
                return null;

            var faceVector = faceResult.Vectors.First().Value;
            var inputEmbedding = JsonSerializer.Deserialize<List<double>>(faceVector.GetRawText());

            if (inputEmbedding == null)
                return null;

            var persons = await _db.Persons
                .Where(p => !string.IsNullOrEmpty(p.FaceEmbedding))
                .ToListAsync();

            double bestScore = double.MaxValue;
            Person? bestMatch = null;

            foreach (var person in persons)
            {
                if (string.IsNullOrWhiteSpace(person.FaceEmbedding)) continue;

                var dbEmbedding = JsonSerializer.Deserialize<List<double>>(person.FaceEmbedding);
                if (dbEmbedding == null)
                    continue;

                double distance = ComputeEuclideanDistance(inputEmbedding, dbEmbedding);
                if (distance < bestScore)
                {
                    bestScore = distance;
                    bestMatch = person;
                }
            }

            return bestMatch;
        }

        // ✅ دالة لحساب المسافة بين المتجهات
        private double ComputeEuclideanDistance(List<double> v1, List<double> v2)
        {
            if (v1.Count != v2.Count) return double.MaxValue;

            double sum = 0;
            for (int i = 0; i < v1.Count; i++)
            {
                double diff = v1[i] - v2[i];
                sum += diff * diff;
            }

            return Math.Sqrt(sum);
        }
    }
}
