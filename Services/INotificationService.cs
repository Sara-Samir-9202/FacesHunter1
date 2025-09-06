using System.Threading.Tasks;

namespace FacesHunter.Services
{
    public interface INotificationService
    {
        Task SendAsync(int receiverUserId, string message, int personId, int? senderUserId = null);
    }
}
