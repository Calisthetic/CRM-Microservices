using System.ComponentModel.DataAnnotations;

namespace UsersAPI.Models.Incoming.Users
{
    public class TokenRequestDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
