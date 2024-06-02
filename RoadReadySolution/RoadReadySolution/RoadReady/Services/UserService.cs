using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;

namespace RoadReady.Services
{
    public class UserService : IUserUserService, IUserAdminService
    {
        private readonly IRepository<int, User> _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IRepository<int, User> userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        #region ---> AddUser
        public async Task<User> AddUser(User user)
        {
            try
            {
                return await _userRepository.Add(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in AddUser: {ex.Message}");
                throw;
            }
        }
        #endregion

        public async Task<User> DeleteUser(int userId)
        {
            try
            {
                return await _userRepository.Delete(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteUser: {ex.Message}");
                throw;
            }
        }

        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                return await _userRepository.GetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAllUsers: {ex.Message}");
                throw;
            }
        }

        public async Task<User> GetUserById(int userId)
        {
            try
            {
                return await _userRepository.GetAsyncById(userId) ?? throw new NoSuchUserException();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetUserById: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Payment>> GetUserPayments(int userId)
        {
            try
            {
                var user = await _userRepository.GetAsyncById(userId) ?? throw new NoSuchUserException();
                return user.Payments?.ToList() ?? new List<Payment>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetUserPayments: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Reservation>> GetUserReservations(int userId)
        {
            try
            {
                var user = await _userRepository.GetAsyncById(userId) ?? throw new NoSuchUserException();
                return user.Reservations?.ToList() ?? new List<Reservation>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetUserReservations: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Review>> GetUserReviews(int userId)
        {
            try
            {
                var user = await _userRepository.GetAsyncById(userId) ?? throw new NoSuchUserException();
                return user.Reviews?.ToList() ?? new List<Review>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetUserReviews: {ex.Message}");
                throw;
            }
        }

        public async Task<User> UpdateEmail(int userId, string email)
        {
            try
            {
                User user = await _userRepository.GetAsyncById(userId);

                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    throw new NoSuchUserException();
                }

                user.Email = email;

                await _userRepository.Update(user);

                return user;
            }
            catch (NoSuchUserException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating email for user with ID {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<User> UpdatePassword(int userId, byte[] password)
        {
            try
            {
                User user = await _userRepository.GetAsyncById(userId);

                if (user == null)
                {
                    throw new NoSuchUserException();
                }

                user.Password = password;

                await _userRepository.Update(user);

                return user;
            }
            catch (NoSuchUserException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating password for user with ID {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<User> UpdatePhoneNumber(int userId, string phoneNumber)
        {
            try
            {
                User user = await _userRepository.GetAsyncById(userId);

                if (user == null)
                {
                    throw new NoSuchUserException();
                }

                user.PhoneNumber = phoneNumber;

                await _userRepository.Update(user);

                return user;
            }
            catch (NoSuchUserException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating phone number for user with ID {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<User> UpdateUserName(int userId, string userName)
        {
            try
            {
                User user = await _userRepository.GetAsyncById(userId);

                if (user == null)
                {
                    throw new NoSuchUserException();
                }

                user.Username = userName;

                await _userRepository.Update(user);

                return user;
            }
            catch (NoSuchUserException ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating username for user with ID {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<User> GetUserByName(string userName)
        {
            try
            {
                return await _userRepository.GetAsyncByName(userName) ?? throw new NoSuchUserException();

            }
            catch(Exception ex)
            {
                _logger.LogError($"Error in GetUserByName:{ex.Message}");
                throw;
            }
        }
    }
}
