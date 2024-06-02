using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using RoadReady.Models.DTO;
using System.Data;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("ReactPolicy")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationAdminService _reservationService;
        public ReservationController(IReservationAdminService reservationAdminService)
        {
            _reservationService = reservationAdminService;
        }
        //Admin Action
        [Authorize(Roles = "admin")]
        [HttpGet("admin/All")]
        public async Task<ActionResult<List<Reservation>>> GetAllReservations()
        {
            try
            {
                var reservations = await _reservationService.GetAllReservations();
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error in GetAllReservations: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin/pending")]
        public async Task<ActionResult<List<Reservation>>> GetPendingReservations()
        {
            try
            {
                var pendingReservations = await _reservationService.GetPendingReservations();
                if (pendingReservations == null || pendingReservations.Count == 0)
                {
                    return NotFound("No pending reservations found.");
                }
                return Ok(pendingReservations);
            }
            catch (NoPendingReservationsException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving pending reservations: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("admin/{reservationId}/update-status")]
        public async Task<ActionResult<Reservation>> UpdateReservationStatus(ReservationStatusDto reservationStatusDto)
        {
            try
            {
                var updatedStatus = await _reservationService.UpdateReservationStatus(reservationStatusDto.ReservationId, reservationStatusDto.Status);
                return Ok(updatedStatus); 


            }
            catch (NoSuchReservationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("admin/{reservationId}/update-price")]
        public async Task<ActionResult<Reservation>> UpdateReservationPrice(ReservationPriceDto reservationPriceDto)
        {
            try
            {
                var updatedPrice = await _reservationService.UpdateReservationPrice(reservationPriceDto.ReservationId, reservationPriceDto.TotalPrice);

                return Ok(updatedPrice);
            }
            catch (NoSuchReservationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        //User Action
        [Authorize(Roles = "user,admin")]
        [HttpGet("user/{id}")]
        public async Task<ActionResult<Reservation>> GetReservationDetails(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservationDetails(id);
                if (reservation != null)
                {
                    return Ok(reservation);
                }
                else
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting reservation details: {ex.Message}");
            }
        }

        [Authorize(Roles = "user")]
        [HttpGet("user/{id}/status")]
        public async Task<ActionResult<Reservation>> GetReservationStatus(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservsationStatus(id);
                if (reservation != null)
                {
                    return Ok(reservation);
                }
                else
                {
                    return NotFound($"Reservation with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting reservation status: {ex.Message}");
            }
        }

        [Authorize(Roles = "user")]
        [HttpPost("user/MakeReservation")]
        public async Task<ActionResult<Reservation>> CreateReservation(Reservation reservation)
        {
            try
            {
                var createdReservation = await _reservationService.CreateReservation(reservation);
                return Ok(createdReservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating reservation: {ex.Message}");
            }
        }

        [Authorize(Roles = "user,admin")]
        [HttpGet("user/Admin/{userId}")]
        public async Task<ActionResult<List<Reservation>>> GetUserReservations(int userId)
        {
            try
            {
                var userReservations = await _reservationService.GetUserReservations(userId);
                if (userReservations != null && userReservations.Any())
                {
                    return Ok(userReservations);
                }
                else
                {
                    return NotFound($"No reservations found for user with ID {userId}.");
                }
            }
            catch (NoSuchReservationException ex)
            {
                return NotFound($"No reservations found for the user: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting user reservations: {ex.Message}");
            }
        } 
        
        [Authorize(Roles = "user,admin")]
        [HttpGet("user/Admin/ Car/{carId}")]
        public async Task<ActionResult<List<Reservation>>> GetReservationsByCarId(int carId)
        {
            try
            {
                var userReservations = await _reservationService.GetReservationsByCarId(carId);
                if (userReservations != null && userReservations.Any())
                {
                    return Ok(userReservations);
                }
                else
                {
                    return NotFound($"No reservations found for user with ID {carId}.");
                }
            }
            catch (NoSuchReservationException ex)
            {
                return NotFound($"No reservations found for the user: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting user reservations: {ex.Message}");
            }
        }

        [Authorize(Roles = "user")]
        [HttpPut("user/{reservationId}/cancel")]
        public async Task<ActionResult<bool>> CancelReservation(int reservationId)
        {
            try
            {
                var success = await _reservationService.CancelReservation(reservationId);
                if (success)
                {
                    return Ok($"Reservation with ID {reservationId} has been cancelled.");
                }
                else
                {
                    return NotFound($"Reservation with ID {reservationId} not found.");
                }
            }
            catch (NoSuchReservationException ex)
            {
                return NotFound($"No such reservation found: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while canceling the reservation: {ex.Message}");
            }
        }
    }
}
