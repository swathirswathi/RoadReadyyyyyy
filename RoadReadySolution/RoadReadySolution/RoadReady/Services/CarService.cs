using Microsoft.EntityFrameworkCore;
using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using RoadReady.Repositories;

namespace RoadReady.Services
{
    public class CarService : ICarAdminService,ICarUserService
    {
        private readonly IRepository<int, Car> _carRepository;
        private readonly IRepository<int, Discount> _discountRepository;
        private readonly ILogger<CarService> _logger;
        public CarService(IRepository<int, Car> carRepository, IRepository<int, Discount> discountRepository, ILogger<CarService> logger)
        {
            _discountRepository=discountRepository;
            _carRepository = carRepository;
            _logger = logger;
        }

        #region AddCar
        public async Task<Car> AddCar(Car car)
        {
            try
            {
                var cars = await _carRepository.Add(car);
                return cars;
            }
            catch (CarAlreadyExistsException ex)
            {
                _logger.LogWarning($"Car with ID {car.CarId} already exists.{ex.Message}");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding the car: {ex.Message}");
                throw; // Re-throw the exception
            }
        }
        #endregion

        #region AddDiscountToCar
        public async Task<Car> AddDiscountToCar(int carId, int discountId)
        {
                try
                {
                    var existingDiscount = await _discountRepository.GetAsyncById(discountId);
                    var car = await _carRepository.GetAsyncById(carId);

                    if (existingDiscount != null && car != null)
                    {
                        car.Discounts.Add(existingDiscount);
                        car=await _carRepository.Update(car);
                         // Save changes to the database
                        return car;
                    }
                    else
                    {
                        // Handle case where discount or car is not found
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception
                    return null;
                }
                
            }
        #endregion

        #region DeleteCar
        public async Task<Car> DeleteCar(int id)
        {
            try
            {
                return await _carRepository.Delete(id);
            }
            catch (NoSuchCarException ex)
            {
                _logger.LogWarning($"Car with ID {id} not found.");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting the car: {ex.Message}");
                throw; // Re-throw the exception
            }
        }
        #endregion

        #region GetCarById
        public async Task<Car> GetCarById(int id)
        {
            try
            {
                return await _carRepository.GetAsyncById(id);
            }
            catch (NoSuchCarException)
            {
                _logger.LogWarning($"Car with ID {id} not found.");
                throw; // Re-throw the exception
            }
           catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting the car by ID: {ex.Message}");
                throw ex; // Re-throw the exception
            }
        }
        #endregion

        public async Task<List<Car>> GetCarsByAvailabilityStatus()
        {
            List<Car> allCars = await _carRepository.GetAsync();
            List<Car> availableCars = allCars.Where(c => c.Availability == true).ToList();
            return availableCars;
        }

        public async Task<List<Car>> GetCarsList()
        {
            try
            {
                var cars = await _carRepository.GetAsync();
                if (cars == null || cars.Count == 0)
                {
                    throw new CarListEmptyException();
                }
                return cars;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting the list of cars: {ex.Message}");
                throw; // Re-throw the exception
            }
        }

        public async Task<Reservation> MakeReservation(int carId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var car = await _carRepository.GetAsyncById(carId) ?? throw new NoSuchCarException();

                // Check car availability
                if (car.Availability != true)
                {
                    _logger.LogWarning($"Car with ID {carId} is not available for reservation");
                    throw new NoSuchCarException();
                }

                // Check if the requested dates are valid
                if (startDate >= endDate)
                {
                    _logger.LogWarning($"Invalid reservation dates. Start date must be before end date.");
                    throw new InvalidReservationDatesException();
                }

                // Check if there are overlapping reservations
                if (car.Reservations.Any(reservation =>
                    (startDate >= reservation.PickUpDateTime && startDate <= reservation.DropOffDateTime) ||
                    (endDate >= reservation.PickUpDateTime && endDate <= reservation.DropOffDateTime)))
                {
                    _logger.LogWarning($"There is already a reservation for the specified dates on car with ID {carId}");
                    throw new ReservationConflictException();
                }

                // Create a new reservation
                var reservation = new Reservation
                {
                    CarId = carId,
                    PickUpDateTime = startDate,
                    DropOffDateTime = endDate
                };

                // Add the reservation to the car
                car.Reservations.Add(reservation);

                // Update the car in the repository
                await _carRepository.Update(car);

                return reservation;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in MakeReservation: {ex.Message}");
                throw;
            }
        }

        public async Task<Car> RemoveDiscountFromCar(int carId, int discountId)
        {
            try
            {
                var car = await _carRepository.GetAsyncById(carId) ?? throw new NoSuchCarException();

                // Check if the car has the specified discount
                var discountToRemove = car.Discounts.FirstOrDefault(d => d.DiscountId == discountId);
                if (discountToRemove != null)
                {
                    // Remove the discount from the car
                    car.Discounts.Remove(discountToRemove);

                    // Update the car in the repository
                    await _carRepository.Update(car);

                    return car;
                }
                else
                {
                    _logger.LogWarning($"The car with ID {carId} does not have the specified discount with ID {discountId}");
                    throw new DiscountNotAssignedToCarException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in RemoveDiscountFromCar: {ex.Message}");
                throw;
            }
        }

        public async Task<Car> UpdateCarDailyRate(int carid,double DailyRate)
        {
            try
            {
                var car = await _carRepository.GetAsyncById(carid);
                if (car != null)
                {
                    car.DailyRate = DailyRate;
                    return await _carRepository.Update(car);
                }
                return null;
            }
            catch (NoSuchCarException ex)
            {
                _logger.LogWarning($"Car with ID {carid} not found.");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the car details: {ex.Message}");
                throw; // Re-throw the exception
            }
        } 

        public async Task<Car> UpdateCarAvailibility(int carid,bool Availability)
        {
            try
            {
                var car = await _carRepository.GetAsyncById(carid);
                if (car != null)
                {
                    car.Availability = Availability;
                    return await _carRepository.Update(car);
                }
                return null;
            }
            catch (NoSuchCarException ex)
            {
                _logger.LogWarning($"Car with ID {carid} not found.");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the car details: {ex.Message}");
                throw; // Re-throw the exception
            }
        }

        public async Task<Car> UpdateCarSpecification(int carid, string Specification)
        {
            try
            {
                var car = await _carRepository.GetAsyncById(carid);
                if (car != null)
                {
                    car.Specification = Specification;
                    return await _carRepository.Update(car);
                }
                return null;
            }
            catch (NoSuchCarException ex)
            {
                _logger.LogWarning($"Car with ID {carid} not found.");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the car details: {ex.Message}");
                throw; // Re-throw the exception
            }
        }

        public async Task<List<Reservation>> ViewAllReservations()
        {
            try
            {
                var allCars = await _carRepository.GetAsync();
                var allReservations = allCars.SelectMany(car => car.Reservations).ToList();
                return allReservations;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewAllReservations: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Car>> ViewAvailableCars(DateTime startDate, DateTime endDate)
        {
            try
            {
                var allCars = await _carRepository.GetAsync();

                // Filter available cars based on reservations and dates

                var availableCars = allCars
                    .Where(car => car.Availability == true && // Check if the car is available
                        (car.Reservations == null || // Check if reservations collection is null
                             !car.Reservations.Any(reservation => // Check if any reservation overlaps
                          reservation.DropOffDateTime <= endDate && reservation.PickUpDateTime >= startDate))).ToList();

                return availableCars;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewAvailableCars: {ex.Message}");
                throw;
            }
        }
      
        public async Task<bool> ViewCarAvailability(int carId)
        {
            try
            { 
                var car = await _carRepository.GetAsyncById(carId) ?? throw new NoSuchCarException();

                // Check if the car is available
                if (car.Availability ?? false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewCarAvailability: {ex.Message}");
                throw;
            }
        }

        public async Task<Car> ViewCarDetails(int carId)
        {
            try
            {
                var car = await _carRepository.GetAsyncById(carId) ?? throw new NoSuchCarException();

                // Return the details of the car
                return car;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewCarDetails: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Reservation>> ViewPastReservations(int userId)
        {
            try
            {
                var allCars = await _carRepository.GetAsync();
                var pastReservations = allCars.SelectMany(car => car.Reservations)
                                              .Where(reservation => reservation.UserId == userId && reservation.DropOffDateTime < DateTime.Now)
                                              .ToList();

                return pastReservations;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewPastReservations: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Payment>> ViewPayments(int userId)
        {
            try
            {
                var allCars = await _carRepository.GetAsync();
                var payments = allCars.SelectMany(car => car.Payments)
                                      .Where(payment => payment.UserId == userId)
                                      .ToList();

                return payments;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewPayments: {ex.Message}");
                throw;
            }
        }

        public async Task<Reservation> ViewReservationDetails(int reservationId)
        {
            try
            {
                var allCars = await _carRepository.GetAsync();
                var reservation = allCars.SelectMany(car => car.Reservations)
                                        .FirstOrDefault(res => res.ReservationId == reservationId);

                if (reservation != null)
                {
                    return reservation;
                }
                else
                {
                    _logger.LogWarning($"Reservation with ID {reservationId} not found");
                    throw new NoSuchReservationException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewReservationDetails: {ex.Message}");
                throw;
            }
        }

        public async Task<Reservation> ViewReservationDetailsForAdmin(int reservationId)
        {
            try
            {
                var allCars = await _carRepository.GetAsync();
                var reservation = allCars.SelectMany(car => car.Reservations)
                                        .FirstOrDefault(res => res.ReservationId == reservationId);

                if (reservation != null)
                {
                    return reservation;
                }
                else
                {
                    _logger.LogWarning($"Reservation with ID {reservationId} not found");
                    throw new NoSuchReservationException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewReservationDetailsForAdmin: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Review>> ViewReviews(int carId)
        {
            try
            {
                var car = await _carRepository.GetAsyncById(carId) ?? throw new NoSuchCarException();

                // Return the reviews for the specified car
                return car.Reviews.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewReviews: {ex.Message}");
                throw;
            }
        }
    }
}
