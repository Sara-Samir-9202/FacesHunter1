
// Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace FacesHunter.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
       
        
        
        [Required]
        public string Role { get; set; } = "User"; // Default role is "User"
        public bool IsVerified { get; set; }
    }
}
