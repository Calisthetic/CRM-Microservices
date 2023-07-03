namespace UsersAPI.Models.DTOs.Incoming.Users
{
    public class UserLoginRequestDto
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
