﻿using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using RoadReady.Repositories;

namespace RoadReady.Services
{
    public class DiscountService : IDiscountAdminService
    {

        private readonly IRepository<int, Discount> _discountRepository;
        private readonly IRepository<int, Reservation> _reservationRepository;
        private readonly IRepository<int, Car> _carRepository;
        private readonly ILogger<DiscountService> _logger;
        public DiscountService(IRepository<int, Discount> discountRepository, IRepository<int, Reservation> reservationRepository, IRepository<int, Car> carRepository, ILogger<DiscountService> logger)
        {
            _discountRepository = discountRepository;
            _reservationRepository = reservationRepository;
            _carRepository = carRepository;
            _logger = logger;
        }

        public async Task<Discount> AddNewDiscount(Discount discount)
        {
            try
            {
                // Implement logic to add a new discount
                return await _discountRepository.Add(discount);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in AddNewDiscount: {ex.Message}");
                throw;
            }
        }

        public async Task<Reservation> ApplyDiscountToReservation(int reservationId, string discountCode)
        {
            try
            {
                // Implement logic to apply a discount to a reservation
                var reservation = await _reservationRepository.GetAsyncById(reservationId);
                var discount = await _discountRepository.GetAsync(); // Retrieve discount by code or other criteria

                // Apply discount logic here

                // Update reservation with applied discount
                reservation.AppliedDiscounts = discount;
                await _reservationRepository.Update(reservation);

                return reservation;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ApplyDiscountToReservation: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> AssignDiscountToCar(int discountId, int carId)
        {
            try
            {
                // Implement logic to assign a discount to a car
                var existingDiscount = await _discountRepository.GetAsyncById(discountId);
                var car = await _carRepository.GetAsyncById(carId);

                if (existingDiscount != null && car != null)
                {
                    car.Discounts.Add(existingDiscount);
                    await _carRepository.Update(car);
                    return true;
                }
                else
                {
                    throw new NoSuchDiscountException();
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error in AssignDiscountToCar: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeactivateDiscount(int discountId)
        {
            try
            {
                // Implement logic to deactivate a discount
                var existingDiscount = await _discountRepository.GetAsyncById(discountId);
                if (existingDiscount != null)
                {
                    existingDiscount.EndDateOfDiscount = DateTime.Now; // Set end date to current date to deactivate
                    await _discountRepository.Update(existingDiscount);
                    return true;
                }
                else
                {
                    throw new NoSuchDiscountException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeactivateDiscount: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> RemoveDiscountFromCar(int discountId, int carId)
        {
            try
            {
                // Implement logic to remove a discount from a car
                var existingDiscount = await _discountRepository.GetAsyncById(discountId);
                var car = await _carRepository.GetAsyncById(carId);

                if (existingDiscount != null && car != null)
                {
                    // Check if the car has the discount before removing
                    if (car.Discounts.Contains(existingDiscount))
                    {
                        // Remove the discount from the car
                        car.Discounts.Remove(existingDiscount);

                        // Update the car in the repository
                        await _carRepository.Update(car);

                        return true;
                    }
                    else
                    {
                        // The car does not have the specified discount
                        throw new DiscountNotAssignedToCarException();
                    }
                }
                else
                {
                    // Discount or car not found
                    throw new NoSuchDiscountException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in RemoveDiscountFromCar: {ex.Message}");
                throw;
            }
        }

        public async Task<Discount> UpdateDiscountEndDate(int discountId, DateTime endDate)
        {
            try
            {
                // Validate inputs
                if (endDate <= DateTime.Now)
                {
                    throw new ArgumentException("End date must be in the future.", nameof(endDate));
                }

                // Retrieve the discount entity from the repository
                Discount discountToUpdate = await _discountRepository.GetAsyncById(discountId);

                if (discountToUpdate == null)
                {
                    _logger.LogWarning($"Discount with ID {discountId} not found.");
                    throw new NoSuchDiscountException();
                }

                // Update the end date
                discountToUpdate.EndDateOfDiscount = endDate;

                // Save the changes to the repository
                await _discountRepository.Update(discountToUpdate);

                return discountToUpdate;
            }
            catch (NoSuchDiscountException ex)
            {
                _logger.LogWarning($"Discount with ID {discountId} not found.");
                throw; // Re-throw the exception
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Invalid argument: {ex.Message}");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the discount end date: {ex.Message}");
                throw; // Re-throw the exception
            }
        }

        public async Task<Discount> UpdateDiscountPercentage(int discountId, double percentage)
        {
            try
            {
                // Validate inputs
                if (percentage < 0 || percentage > 100)
                {
                    throw new ArgumentException("Percentage must be between 0 and 100.", nameof(percentage));
                }

                // Retrieve the discount entity from the repository
                Discount discountToUpdate = await _discountRepository.GetAsyncById(discountId);

                if (discountToUpdate == null)
                {
                    _logger.LogWarning($"Discount with ID {discountId} not found.");
                    throw new NoSuchDiscountException();
                }

                // Update the percentage
                discountToUpdate.DiscountPercentage = percentage;

                // Save the changes to the repository
                await _discountRepository.Update(discountToUpdate);

                return discountToUpdate;
            }
            catch (NoSuchDiscountException ex)
            {
                _logger.LogWarning($"Discount with ID {discountId} not found.");
                throw; // Re-throw the exception
            }
            catch (ArgumentException ex)
            {
                _logger.LogError($"Invalid argument: {ex.Message}");
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the discount percentage: {ex.Message}");
                throw; // Re-throw the exception
            }
        }


        public async Task<List<Discount>> ViewAllDiscounts()
        {
            try
            {
                // Implement logic to retrieve all discounts
                return await _discountRepository.GetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewAllDiscounts: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Discount>> ViewAppliedDiscounts(int reservationId)
        {
            try
            {
                // Implement logic to retrieve applied discounts for a reservation
                var reservation = await _reservationRepository.GetAsyncById(reservationId);
                return (List<Discount>)reservation.AppliedDiscounts;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewAppliedDiscounts: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Discount>> ViewAvailableDiscounts()
        {
            try
            {
                // Implement logic to retrieve active discounts
                var allDiscounts = await _discountRepository.GetAsync();
                var currentDate = DateTime.Now;

                // Filter active discounts based on the current date
                var activeDiscounts = allDiscounts
                    .Where(discount => discount.StartDateOfDiscount <= currentDate && currentDate <= discount.EndDateOfDiscount)
                    .ToList();

                return activeDiscounts;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewAvailableDiscounts: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Discount>> ViewCarsWithDiscounts()
        {
            try
            {

                // Implement logic to retrieve cars with associated discounts
                var discounts = await _discountRepository.GetAsync();

                // Filter discounts to include only those with non-null carId
                var discountsWithNonNullCarId = discounts.Where(d => d.CarId != null).ToList();

                // Flatten the list of cars from discounts and remove duplicates
                


                return discountsWithNonNullCarId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewCarsWithDiscounts: {ex.Message}");
                throw;
            }
        }

        public async Task<Discount> ViewDiscountDetails(int discountId)
        {
            try
            {
                // Implement logic to retrieve details of a discount
                return await _discountRepository.GetAsyncById(discountId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewDiscountDetails: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Car>> ViewDiscountedCars()
        {
            try
            {
                // Implement logic to retrieve cars with associated discounts
                var discounts = await _discountRepository.GetAsync();
                var discountedCars = discounts.SelectMany(d => d.Cars).Distinct().ToList();
                return discountedCars;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ViewDiscountedCars: {ex.Message}");
                throw;
            }
        }
    }
}
