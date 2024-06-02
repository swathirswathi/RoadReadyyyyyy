using Microsoft.Win32;
using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using System.Text;

namespace RoadReady.Services
{
        public class AdminService : IAdminService
        {
            IRepository<int, Admin> _repo;
            ILogger<AdminService> _logger;
            public AdminService(IRepository<int, Admin> repo, ILogger<AdminService> logger)
            {
                _repo = repo;
                _logger = logger;
            }
      
            public async Task<Admin> DeleteAdmin(int adminId)
            {
                try
                {
                    return await _repo.Delete(adminId);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in deleting the admin : {ex.Message}");
                    throw;
                }
            }

            public async Task<Admin> GetAdminById(int adminId)
            {
                try
                {
                    return await _repo.GetAsyncById(adminId) ?? throw new NoSuchAdminException();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in retrieving admin by given id : {ex.Message}");
                    throw;
                }
            }

            public async Task<List<Admin>> GetAllAdmins()
            {
                try
                {
                    return await _repo.GetAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in retrieving admins list : {ex.Message}");
                    throw;
                }
            }

        public async Task<Admin> GetAdminByName(string userName)
        {
            try
            {
                return await _repo.GetAsyncByName(userName) ?? throw new NoSuchAdminException();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in retrieving admin by given Name : {ex.Message}");
                throw;
            }
        }

        public async Task<Admin> UpdateAdminEmail(int adminId, string email)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email cannot be null or empty.", nameof(email));
                }


                Admin adminToUpdate = await _repo.GetAsyncById(adminId);

                if (adminToUpdate == null)
                {
                    _logger.LogWarning($"Admin with ID {adminId} not found.");
                    throw new NoSuchAdminException();
                }

                // Update the email
                adminToUpdate.Email = email;

                // Save the changes to the database 
                await _repo.Update(adminToUpdate);

                return adminToUpdate;
            }
            catch (NoSuchAdminException ex)
            {
                _logger.LogWarning($"Admin with ID {adminId} not found.");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the admin email: {ex.Message}");
                throw; // Re-throw the exception
            }
        }

        public async Task<Admin> UpdateAdminPassword(int adminId, byte[] password)
        {
            try
            {
                // Validate inputs
                if (password == null || password.Length == 0)
                {
                    throw new ArgumentException("Password cannot be null or empty.", nameof(password));
                }

                // Retrieve the admin entity from the database 
                Admin adminToUpdate = await _repo.GetAsyncById(adminId);

                if (adminToUpdate == null)
                {
                    _logger.LogWarning($"Admin with ID {adminId} not found.");
                    throw new NoSuchAdminException();
                }

                // Update the password
                adminToUpdate.Password = password;
                
                // Save the changes to the database 
                await _repo.Update(adminToUpdate);

                return adminToUpdate;
            }
            catch (NoSuchAdminException ex)
            {
                _logger.LogWarning($"Admin with ID {adminId} not found.");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the admin password: {ex.Message}");
                throw; // Re-throw the exception
            }
        }
        public async Task<Admin> UpdateAdminPhoneNumber(int adminId, string phoneNumber)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(phoneNumber))
                {
                    throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));
                }

                // Retrieve the admin entity from the database 
                Admin adminToUpdate = await _repo.GetAsyncById(adminId);

                if (adminToUpdate == null)
                {
                    _logger.LogWarning($"Admin with ID {adminId} not found.");
                    throw new NoSuchAdminException();
                }

                // Update the phone number
                adminToUpdate.PhoneNumber = phoneNumber;

                // Save the changes to the database
                await _repo.Update(adminToUpdate);

                return adminToUpdate;
            }
            catch (NoSuchAdminException ex)
            {
                _logger.LogWarning($"Admin with ID {adminId} not found.");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the admin phone number: {ex.Message}");
                throw; // Re-throw the exception
            }
        }
        public async Task<Admin> UpdateAdminUserName(int adminId, string username)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("Username cannot be null or empty.", nameof(username));
                }

                // Retrieve the admin entity from the database (assuming you have a data access layer)
                Admin adminToUpdate = await _repo.GetAsyncById(adminId);

                if (adminToUpdate == null)
                {
                    _logger.LogWarning($"Admin with ID {adminId} not found.");
                    throw new NoSuchAdminException();
                }

                // Update the username
                adminToUpdate.Username = username;

                // Save the changes to the database (assuming you have a data access layer)
                await _repo.Update(adminToUpdate);

                return adminToUpdate;
            }
            catch (NoSuchAdminException ex)
            {
                _logger.LogWarning($"Admin with ID {adminId} not found.");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the admin username: {ex.Message}");
                throw; // Re-throw the exception
            }
        }
    }
    }
