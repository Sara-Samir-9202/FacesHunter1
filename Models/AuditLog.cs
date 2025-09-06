

//Models/ AuditLog.ca

using System;

namespace FacesHunter.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty; // مثال: Update, Delete
        public string EntityName { get; set; } = string.Empty; // مثال: Person
        public int EntityId { get; set; }
        public int UserId { get; set; }
        public string Changes { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

