using System.Threading.Tasks;
using FacesHunter.DTOs;

namespace FacesHunter.Services
{
    public interface IResetPasswordService
    {
        Task<(bool Success, string Message)> SendResetCodeAsync(string email);
        Task<(bool Success, string Message)> ResetPasswordAsync(string email, string code, string newPassword);
    }
}

