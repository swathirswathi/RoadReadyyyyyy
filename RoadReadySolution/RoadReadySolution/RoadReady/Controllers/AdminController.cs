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
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
          
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
            
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("admin/admins/del/{id}")]
        public async Task<ActionResult<Admin>> DeleteAdmin(int id)
        {
            try
            {
               var admin=await _adminService.DeleteAdmin(id);
                return Ok("Admin deleted successfully");
            }
            catch (NoSuchAdminException)
            {
                return NotFound($"Admin with ID {id} not found.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the Admin.");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin/admins/get/{userName}")]
        public async Task<ActionResult<Admin>> GetAdminByName(string userName)
        {
            try
            {
                var admin = await _adminService.GetAdminByName(userName);
                if (admin == null)
                {
                    return NotFound($"Admin with Name {userName} not found.");
                }
                return Ok(admin);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while fetching the admin details.");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin/admins/{id}")]
        public async Task<ActionResult<Admin>> GetAdminById(int id)
        {
            try
            {
                var admin = await _adminService.GetAdminById(id);
                if (admin == null)
                {
                    return NotFound($"Admin with ID {id} not found.");
                }
                return Ok(admin);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while fetching the admin details.");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin/GetAllAdmin")]
        public async Task<ActionResult<List<Admin>>> GetAllAdmins()
        {
            try
            {
                //OK--200(Success)
                return Ok(await _adminService.GetAllAdmins());
            }
            catch (AdminListEmptyException)
            {
                return NotFound("Admin List is Empty.");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{adminId}/update-email")]
        public async Task<ActionResult<Admin>> UpdateAdminEmail(AdminEmailDto adminEmailDto)
        {
            try
            {
                var updatedAdmin = await _adminService.UpdateAdminEmail(adminEmailDto.AdminId, adminEmailDto.Email);
                return Ok(updatedAdmin);
            }
            catch (NoSuchAdminException ex)
            {
                return NotFound($"Admin with ID {adminEmailDto.AdminId} not found.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{adminId}/update-password")]
        public async Task<ActionResult<Admin>> UpdateAdminPassword(AdminPasswordDto adminPasswordDto)
        {
            try
            {
                var updatedAdmin = await _adminService.UpdateAdminPassword(adminPasswordDto.AdminId, adminPasswordDto.Password);
                return Ok(updatedAdmin);
            }
            catch (NoSuchAdminException ex)
            {
                return NotFound($"Admin with ID {adminPasswordDto.AdminId} not found.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{adminId}/update-phone-number")]
        public async Task<ActionResult<Admin>> UpdateAdminPhoneNumber(AdminPhoneNumberDto adminPhoneNumberDto)
        {
            try
            {
                var updatedAdmin = await _adminService.UpdateAdminPhoneNumber(adminPhoneNumberDto.AdminId, adminPhoneNumberDto.PhoneNumber);
                return Ok(updatedAdmin);
            }
            catch (NoSuchAdminException ex)
            {
                return NotFound($"Admin with ID {adminPhoneNumberDto.AdminId} not found.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        
    }
}
