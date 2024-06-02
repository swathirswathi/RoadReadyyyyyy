using RoadReady.Models;

namespace RoadReady.Interface
{
    public interface IUserAdminService
    {
        Task<List<User>> GetAllUsers();
        Task<User> AddUser(User user);
        Task<User> DeleteUser(int userId);
    }
}
