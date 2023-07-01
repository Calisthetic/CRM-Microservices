namespace UsersAPI.Models.DTOs.Incoming
{
    public class UserAddNewDto
    {
        public string FirstName { get; set; } = null!;

        public string SecondName { get; set; } = null!;

        public string? ThirdName { get; set; }

        public string? PhoneNumber { get; set; }

        public string Email { get; set; } = null!;

        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int DivisionId { get; set; }

        public int CompanyId { get; set; }

        public int RoleId { get; set; }

        public short? VacationCount { get; set; }
    }
}
