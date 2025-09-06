
    // Models/SuccessStory.cs
    namespace FacesHunter.Models
    {
        public class SuccessStory
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty; 
            public string Description { get; set; } = string.Empty;
            public string VideoPath { get; set; } = string.Empty; // /success_videos/xxx.mp4
            public DateTime DatePosted { get; set; } = DateTime.Now;
        
    }
    }

