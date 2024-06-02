using RoadReady.Models.DTO;

namespace RoadReady.Interface
{
    public interface IValidationService
    {
        public Task<LoginValidationDto> Login(LoginValidationDto validation);
        public Task<LoginValidationDto> RegisterUser(RegisterUserDto validation);
        public Task<LoginValidationDto> RegisterAdmin(RegisterAdminDto validation);
        
    }
}
