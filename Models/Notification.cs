
//Models/Notification.cs

using System;
namespace FacesHunter.Models
{
    public class Notification
    {
        public int Id { get; set; }
        
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? PersonId { get; set; }
        public bool IsRead { get; set; } = false;


        // المستخدم المستلم (nullable لو إشعار عام)
        public int? ReceiverUserId { get; set; }
        public User? ReceiverUser { get; set; }

        // من أرسل الإشعار (ممكن null لو نظام آلي)
        public int? SenderUserId { get; set; }
        public User? SenderUser { get; set; }

      
        }
    }







    

      
