namespace UsersAPI.Models.DTOs.Incoming
{
    public class UserLoginRequestDto
    {
        public string Login { get; set; } = null!; 
        public string Password { get; set; } = null!;
    }
}
