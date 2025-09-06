//Services/FaceSearchService.cs
using FacesHunter.Data;
using FacesHunter.Models;
using System.Text.Json;

public class FaceSearchService
{
    private readonly AppDbContext _context;

    public FaceSearchService(AppDbContext context)
    {
        _context = context;
    }

    public int? FindClosestMatch(List<double> targetEmbedding)
    {
        if (targetEmbedding == null || targetEmbedding.Count == 0)
            return null;

        double bestSimilarity = -1;
        int? bestMatchId = null;

        var persons = _context.Persons
            .Where(p => !string.IsNullOrWhiteSpace(p.FaceEmbedding))
            .ToList();

        foreach (var person in persons)
        {
            List<double>? dbEmbedding = null;

            try
            {
                dbEmbedding = JsonSerializer.Deserialize<List<double>>(person.FaceEmbedding!);
            }
            catch
            {
                continue; // تخطي الشخص في حال فشل التحويل
            }

            if (dbEmbedding == null || dbEmbedding.Count != targetEmbedding.Count)
                continue;

            double similarity = CosineSimilarity(targetEmbedding, dbEmbedding);

            if (similarity > bestSimilarity)
            {
                bestSimilarity = similarity;
                bestMatchId = person.Id;
            }
        }

        return bestMatchId;
    }

    private double CosineSimilarity(List<double> a, List<double> b)
    {
        if (a == null || b == null || a.Count != b.Count)
            return 0;

        double dot = 0, normA = 0, normB = 0;

        for (int i = 0; i < a.Count; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        if (normA == 0 || normB == 0)
            return 0;

        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}
