
// Models/Person.cs
namespace FacesHunter.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public int? Age { get; set; }

        public string Status { get; set; } = "Missing";  // Default to "Missing"

        public Gender Gender { get; set; } = Gender.Male;

        public string? ImagePath { get; set; }

        public DateTime DateMissing { get; set; }  // يحدده المستخدم

        public string LastSeenLocation { get; set; } = string.Empty;
        public string? LocationLost { get; set; }

        public DateTime ReportDate { get; set; } = DateTime.Now;

        public bool IsFound { get; set; }
        public string? ReporterName { get; set; } // اسم الشخص اللي أبلغ
        public string? ReporterIdImage { get; set; } // صورة إثبات الهوية
        public string? ReporterPhone { get; set; } // ممكن نستخدمه لاحقًا في SMS Verification
        public bool IsApproved { get; set; } = false;

        public string? ImageFileName { get; set; } // ← Img5.jpg مثلاً
        public string? FaceEmbedding { get; set; }
        public string? ReporterIdImagePath { get; set; }
        public bool? ReporterVerified { get; set; }

        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; }


        public string? AdditionalInfo { get; set; }

       



    }
}

