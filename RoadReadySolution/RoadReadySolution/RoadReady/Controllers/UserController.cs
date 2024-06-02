using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using RoadReady.Models.DTO;
using RoadReady.Services;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("ReactPolicy")]
    public class UserController : ControllerBase
    {
        private readonly IUserAdminService _userAdminService;
        private readonly IUserUserService _userUserService;

        public UserController(IUserAdminService userAdminService, IUserUserService userUserService)
        {
            _userAdminService = userAdminService;
            _userUserService = userUserService;
        }

        //Admin Action
        [Authorize(Roles = "admin")]
        [HttpGet("admin/GetUser")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _userAdminService.GetAllUsers();
                return Ok(users);
            }
            catch (UserListEmptyException)
            {
                return NotFound("User List is Empty.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting all users: {ex.Message}");
            }
        }

        //[Authorize(Roles = "admin,user")]
        [HttpGet("user/GetUser/get/{userName}")]
        public async Task<ActionResult<User>> GetUserByName(string userName)
        {
            try
            {
                var user = await _userUserService.GetUserByName(userName);
                return Ok(user);
            }
            catch (NoSuchUserException)
            {
                return NotFound($"User with ID {userName} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting user by ID: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin,user")]
        [HttpPut("{userId}/update-email")]
        public async Task<ActionResult<User>> UpdateEmail(UserEmailDto userEmailDto)
        {
            try
            {
                var updatedEmail = await _userUserService.UpdateEmail(userEmailDto.UserId, userEmailDto.Email);
                if (updatedEmail != null)
                {
                    return Ok(updatedEmail);
                }
                else
                {
                    return NotFound($"User with ID {userEmailDto.UserId} not found.");
                }
            }
            catch (NoSuchUserException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the Users mail: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin,user")]
        [HttpPut("{userId}/update-password")]
        public async Task<ActionResult<User>> UpdatePassword(UserPasswordDto userPasswordDto)
        {
            try
            {
                var updatedUser = await _userUserService.UpdatePassword(userPasswordDto.UserId, userPasswordDto.Password);
                if (updatedUser != null)
                    return Ok(updatedUser);
                else
                    return NotFound($"User with ID {userPasswordDto.UserId} not found.");
            }
            catch (NoSuchUserException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the Users Password: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin,user")]
        [HttpPut("{userId}/update-phone-number")]
        public async Task<ActionResult<User>> UpdatePhoneNumber(UserPhoneNumberDto userPhoneNumberDto)
        {
            try
            {
                User updatedUser = await _userUserService.UpdatePhoneNumber(userPhoneNumberDto.UserId, userPhoneNumberDto.PhoneNumber);
                if (updatedUser != null)
                    return Ok(updatedUser);
                else
                    return NotFound($"User with ID {userPhoneNumberDto.UserId} not found.");
            }
            catch (NoSuchUserException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the Users PhoneNumber: {ex.Message}");
            }
        }


        [Authorize(Roles = "admin,user")]
        [HttpDelete("admin/{userId}")]
        public async Task<ActionResult<User>> DeleteUser(int userId)
        {
            try
            {
                var deletedUser = await _userAdminService.DeleteUser(userId);
                return Ok("User deleted successfully");
            }
            catch (NoSuchUserException)
            {
                return NotFound($"User with ID {userId} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting user: {ex.Message}");
            }
        }

        //User Action
        //[Authorize(Roles = "user,admin")]
        [HttpGet("user/GetUser/{userId}")]
        public async Task<ActionResult<User>> GetUserById(int userId)
        {
            try
            {
                var user = await _userUserService.GetUserById(userId);
                return Ok(user);
            }
            catch (NoSuchUserException)
            {
                return NotFound($"User with ID {userId} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting user by ID: {ex.Message}");
            }
        }

      

    }
}
