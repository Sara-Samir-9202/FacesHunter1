//Services/ NotificationService
using FacesHunter.Data;
using FacesHunter.Models;
using Microsoft.EntityFrameworkCore;

namespace FacesHunter.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _db;

        public NotificationService(AppDbContext db)
        {
            _db = db;
        }

        public async Task SendAsync(int receiverUserId, string message, int personId, int? senderUserId = null)
        {
            var notification = new Notification
            {
                Id = receiverUserId,
                Message = message,
                PersonId = personId,
                SenderUserId = senderUserId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
        }
    }
}
