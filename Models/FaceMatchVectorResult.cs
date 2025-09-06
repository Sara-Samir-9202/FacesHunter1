// Models/FaceMatchVectorResult.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FacesHunter.Models
{
    public class FaceMatchVectorResult
    {
        // ✅ نسبة التشابه العامة مع أقرب تطابق (إذا موجود)
        [JsonPropertyName("Similarity")]
        public double Similarity { get; set; }

        // ✅ المتجهات الخاصة بكل صورة متطابقة راجعة من FaceNet
        // مثل: "Img5": [vector values], "Img3": [vector values]
        [JsonExtensionData]
        public Dictionary<string, JsonElement>? Vectors { get; set; }
    }
}






