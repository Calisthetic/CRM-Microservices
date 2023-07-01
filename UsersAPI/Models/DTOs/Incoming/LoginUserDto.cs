namespace UsersAPI.Models.DTOs.Incoming
{
    public class LoginUserDto
    {
        public string Login { get; set; } = null!; 
        public string Password { get; set; } = null!;
    }
}
