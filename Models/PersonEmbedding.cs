//Models / PersonEmbedding.cs
using FacesHunter.Models;

namespace FacesHunter.Models
{
    public class PersonEmbedding
    {
        public int PersonId { get; set; }
        public List<double> Embedding { get; set; } = new();
    }
}

