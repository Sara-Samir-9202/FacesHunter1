// Helpers/AiEndpoints.cs
namespace FacesHunter.Helpers
{
    public static class AiEndpoints
    {
        public static string ArcFaceBase = "https://9a43-34-106-61-116.ngrok-free.app";
        public static string SAMBase = "https://XXXX-sam.ngrok-free.app"; // استبدليه بعد تشغيل SAM
        public static string OCRBase = "https://YYYY-ocr.ngrok-free.app"; // استبدليه بعد تشغيل OCR

        public static string ArcFaceEmbedding => $"{ArcFaceBase}/arcface/embedding";
        public static string ArcFaceCompare => $"{ArcFaceBase}/arcface/compare";
        public static string SAMAgeTransform => $"{SAMBase}/sam/age-transform";
        public static string OCRExtractText => $"{OCRBase}/ocr/extract-text";
    }
}

