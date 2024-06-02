namespace RoadReady.Models.DTO
{
    public class RegisterUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime? RegistrationDate { get; set; } = DateTime.Today;
    }
}
