using RoadReady.Models.DTO;

namespace RoadReady.Interface
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(LoginValidationDto validation);
    }
}
