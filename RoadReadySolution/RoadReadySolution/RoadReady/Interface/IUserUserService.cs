using RoadReady.Models;

namespace RoadReady.Interface
{
    public interface IUserUserService
    {
        Task<User> GetUserById(int userId);
        Task<User> GetUserByName(string userName);
        Task<List<Review>> GetUserReviews(int userId);
        Task<List<Reservation>> GetUserReservations(int userId);
        Task<List<Payment>> GetUserPayments(int userId);
        Task<User> UpdateUserName(int userId, string userName);
        Task<User> UpdatePassword(int userId, byte[] password);
        Task<User> UpdateEmail(int userId, string email);
        Task<User> UpdatePhoneNumber(int userId, string phoneNumber);
    }
}
