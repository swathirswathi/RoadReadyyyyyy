using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using static RoadReady.Models.Payment;
using static RoadReady.Models.Reservation;

namespace RoadReady.Services
{
    public class ReservationService : IReservationAdminService
    {
        private readonly IRepository<int, Reservation> _reservationRepository;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(IRepository<int, Reservation> reservationRepository, ILogger<ReservationService> logger)
        {
            _reservationRepository = reservationRepository;
            _logger = logger;
        }

        public async Task<bool> CancelReservation(int reservationId)
        {
            try
            {

                var reservation = await _reservationRepository.GetAsyncById(reservationId);

                if (reservation != null)
                {

                    reservation.Status = "Cancelled";

                    // Save the updated reservation in the repository
                    await _reservationRepository.Update(reservation);

                    return true;
                }
                else
                {
                    _logger.LogInformation($"Reservation with ID {reservationId} not found.");
                    throw new NoSuchReservationException();
                }
            }
            catch (NoSuchReservationException ex)
            {
                _logger.LogInformation($"No such reservation found: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while canceling the reservation: {ex.Message}");
                throw;
            }
        }

        public async Task<Reservation> CreateReservation(Reservation reservation)
        {
            try
            {
                return await _reservationRepository.Add(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in AddReservation: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Reservation>> GetAllReservations()
        {
            try
            {
                return await _reservationRepository.GetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAllReservations: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Reservation>> GetPendingReservations()
        {
            try
            {
                var allReservations = await _reservationRepository.GetAsync();
                var pendingReservations = allReservations.Where(r => r.Status.Equals("Pending")).ToList();

                if (pendingReservations.Any())
                {
                    return pendingReservations;
                }
                else
                {
                    _logger.LogInformation("No pending reservations found.");
                    throw new NoPendingReservationsException();
                }
            }
            catch (NoPendingReservationsException ex)
            {
                _logger.LogInformation($"No pending reservations found: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting pending reservations: {ex.Message}");
                throw;
            }
        }

        public async Task<Reservation> GetReservationDetails(int id)
        {
            try
            {
                return await _reservationRepository.GetAsyncById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetReservsationById: {ex.Message}");
                throw;
            }
        }

        public async Task<Reservation> GetReservsationStatus(int id)
        {
            try
            {
                var reservation = await _reservationRepository.GetAsyncById(id);

                if (reservation != null)
                {
                    reservation.Status = DetermineReservationStatus(reservation);
                    await _reservationRepository.Update(reservation); // Update the reservation with the new status if necessary
                }

                return reservation;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetReservsationStatus: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Reservation>> GetUserReservations(int userId)
        {
            try
            {
                var userReservations = await _reservationRepository.GetAsync();
                var reservations = userReservations.Where(r => r.UserId == userId).ToList();

                if (reservations.Any())
                {
                    return reservations;
                }
                else
                {
                    _logger.LogInformation($"No reservations found for user with ID {userId}.");
                    throw new NoSuchReservationException();
                }
            }
            catch (NoSuchReservationException ex)
            {
                _logger.LogInformation($"No reservations found for the user: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting user reservations: {ex.Message}");
                throw;
            }
        }


        public async Task<List<Reservation>> GetReservationsByCarId(int carId)
        {
            try
            {
                var userReservations = await _reservationRepository.GetAsync();
                var reservations = userReservations.Where(r => r.CarId == carId).ToList();

                if (reservations.Any())
                {
                    return reservations;
                }
                else
                {
                    _logger.LogInformation($"No reservations found for user with ID {carId}.");
                    throw new NoSuchReservationException();
                }
            }
            catch (NoSuchReservationException ex)
            {
                _logger.LogInformation($"No reservations found for the user: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting user reservations: {ex.Message}");
                throw;
            }
        }

        public async Task<Reservation> UpdateReservationPrice(int reservationId, double newPrice)
        {
            try
            {
                // Fetch the reservation from the repository
                Reservation reservation = await _reservationRepository.GetAsyncById(reservationId);

                if (reservation == null)
                {
                    // Log the error and throw an exception
                    _logger.LogError($"Reservation with ID {reservationId} not found.");
                    throw new NoSuchReservationException();
                }

                // Update the reservation price
                reservation.TotalPrice = newPrice;

                // Save changes to the repository
                await _reservationRepository.Update(reservation);

                return reservation;
            }
            catch (NoSuchReservationException ex)
            {
                // Log and re-throw the custom NotFoundException
                _logger.LogError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Log the generic exception
                _logger.LogError($"An error occurred while updating reservation price for ID {reservationId}: {ex.Message}");
                throw; // Re-throw the exception for the caller to handle
            }
        }

        public async Task<Reservation> UpdateReservationStatus(int reservationId, string newStatus)
        {
            try
            {
                // Fetch the reservation from the repository
                Reservation reservation = await _reservationRepository.GetAsyncById(reservationId);

                if (reservation == null)
                {
                    // Log the error and throw an exception
                    _logger.LogError($"Reservation with ID {reservationId} not found.");
                    throw new NoSuchReservationException();
                }

                // Update the reservation status
                reservation.Status = newStatus;

                // Save changes to the repository
                await _reservationRepository.Update(reservation);

                return reservation;
            }
            catch (NoSuchReservationException ex)
            {
                // Log and re-throw the custom NotFoundException
                _logger.LogError(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Log the generic exception
                _logger.LogError($"An error occurred while updating reservation status for ID {reservationId}: {ex.Message}");
                throw; // Re-throw the exception for the caller to handle
            }
        }
        private string DetermineReservationStatus(Reservation reservation)
        {
            if (reservation.Status.Equals("Cancelled"))
            {
                return "Cancelled";
            }
            else
            {
                return "Reserved";
            }
        }

    }
}
