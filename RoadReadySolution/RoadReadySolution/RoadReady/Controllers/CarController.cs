using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using RoadReady.Models.DTO;
using RoadReady.Repositories;
using RoadReady.Services;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("ReactPolicy")]
    public class CarController : ControllerBase
    {
        private readonly ICarAdminService _carAdminService;
        private readonly ICarUserService _carUserService;

        public CarController(ICarAdminService carAdminService, ICarUserService carUserService)
        {
            _carAdminService = carAdminService;
            _carUserService = carUserService;
        }

        // Admin actions
        [Authorize(Roles = "admin,user")]
        [HttpGet("admin/cars/GetCarsList")]
        public async Task<ActionResult<List<Car>>> GetCarsList()
        {
            try
            {
                //OK--200(Success)
                return Ok(await _carAdminService.GetCarsList());
            }
            catch (CarListEmptyException)
            {
                return NotFound("Car List is Empty.");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("admin/cars")]
        public async Task<ActionResult<Car>> AddCar(Car car)
        {
            try
            {
                var cars = await _carAdminService.AddCar(car);
                return Ok(cars);
            }
            catch (CarAlreadyExistsException)
            {
                return Conflict($"Car with ID {car.CarId} already exists.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the car.");
            }

        }

        [Authorize(Roles = "admin")]
        [HttpPut("{carId}/daily-rate")]
        public async Task<ActionResult<Car>> UpdateCarDailyRate(CarDailyRateDto carDailyRateDto)
        {
            try
            {
                var updatedCar = await _carAdminService.UpdateCarDailyRate(carDailyRateDto.CarId, carDailyRateDto.DailyRate);
                if (updatedCar != null)
                {
                    return Ok(updatedCar);
                }
                else
                {
                    return NotFound($"Car with ID {carDailyRateDto.CarId} not found.");
                }
            }
            catch (NoSuchCarException)
            {
                return NotFound($"Car with ID {carDailyRateDto.CarId} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the car's daily rate: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{carId}/availibility")]
        public async Task<ActionResult<Car>> UpdateCarAvailibility(CarAvailibilityDto carAvailibilityDto)
        {
            try
            {
                var updatedCar = await _carAdminService.UpdateCarAvailibility(carAvailibilityDto.CarId, carAvailibilityDto.Availability);
                if (updatedCar != null)
                {
                    return Ok(updatedCar);
                }
                else
                {
                    return NotFound($"Car with ID {carAvailibilityDto.CarId} not found.");
                }
            }
            catch (NoSuchCarException)
            {
                return NotFound($"Car with ID {carAvailibilityDto.CarId} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the car's daily rate: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{carId}/Specification")]
        public async Task<ActionResult<Car>> UpdateCarSpecification(CarSpecificationDto carSpecificationDto)
        {
            try
            {
                var updatedCar = await _carAdminService.UpdateCarSpecification(carSpecificationDto.CarId, carSpecificationDto.Specification);
                if (updatedCar != null)
                {
                    return Ok(updatedCar);
                }
                else
                {
                    return NotFound($"Car with ID {carSpecificationDto.CarId} not found.");
                }
            }
            catch (NoSuchCarException)
            {
                return NotFound($"Car with ID {carSpecificationDto.CarId} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the car's daily rate: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("admin/cars/{id}")]
        public async Task<ActionResult<Car>> DeleteCar(int id)
        {
            try
            {
               var cars=await _carAdminService.DeleteCar(id);
                if (cars != null)
                {
                    return Ok("Car deleted successfully");
                }
                else
                {
                    return NotFound($"Car with ID {id} not found.");
                }
            }
            catch (NoSuchCarException)
            {
                return NotFound($"Car with ID {id} not found.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the car.");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("admin/cars/discountstocar")]
        public async Task<ActionResult<Car>> AddDiscountToCar(int carId, int discountId)
        {
            try
            {

                return Ok(await _carAdminService.AddDiscountToCar(carId, discountId));
            }
            catch (NoSuchCarException)
            {
                return NotFound($"Car with ID {carId} not found.");
            }
            catch (DiscountAlreadyExistsException)
            {
                //409-(Conflict)
                return Conflict($"Discount with ID {discountId} is already applied to the car with ID {carId}");
            }
            //catch (Exception)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding the discount to the car.");
            //}
        }

       

        [Authorize(Roles = "admin,user")]
        [HttpGet("admin/user/cars/{carId}/availability")]
        public async Task<ActionResult<bool>> ViewCarAvailability(int carId)
        {
            try
            {
                return Ok(await _carAdminService.ViewCarAvailability(carId));
            }
            catch (NoSuchCarException)
            {
                return NotFound($"Car with ID {carId} not found.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the car availability.");
            }
        }

       


        [Authorize(Roles = "admin")]
        [HttpGet("admin/Cars/Availability")]
        public async Task<ActionResult<List<Car>>> GetCarsByAvailabilityStatus()
        {
            try
            {
                return Ok(await _carAdminService.GetCarsByAvailabilityStatus());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving available cars: {ex.Message}");
            }
        }


        //User Action
        [Authorize(Roles = "user")]
        [HttpGet("User/Cars/CarDetails/{carId}")]
        public async Task<ActionResult<Car>> GetCarDetails(int carId)
        {
            try
            {
                var car = await _carUserService.ViewCarDetails(carId);
                return Ok(car); // Return 200 OK with car details
            }
            catch (NoSuchCarException)
            {
                return NotFound($"Cars with ID {carId} not found."); // Return 404 Not Found if the car does not exist
            }
            catch
            {
                return StatusCode(500); // Return 500 Internal Server Error for other exceptions
            }
        }

        [Authorize(Roles = "user")]
        [HttpGet("user/cars/available/Dates")]
        public async Task<ActionResult<List<Car>>> GetAvailableCars(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate >= endDate)
                {
                    return BadRequest("Invalid date range. Start date must be before end date.");
                }

                var availableCars = await _carUserService.ViewAvailableCars(startDate, endDate);
                return Ok(availableCars); // Return 200 OK with list of available cars
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving available cars: {ex.Message}");
            }
        }


        [Authorize(Roles = "user,admin")]
        [HttpGet("User/Cars/{id}")]
        public async Task<ActionResult<Car>> GetCarById(int id)
        {
            try
            {
                var car = await _carUserService.GetCarById(id);
                if (car != null)
                {
                    return Ok(car);
                }
                
                return NotFound($"Car with ID {id} not found.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while fetching the car details.");
            }
        }

        

   
    }

}

