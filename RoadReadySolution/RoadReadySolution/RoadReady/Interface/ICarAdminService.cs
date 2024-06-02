using RoadReady.Models;

namespace RoadReady.Interface
{
    public interface ICarAdminService
    {
        public Task<List<Car>> GetCarsList();
        public Task<Car> AddCar(Car car);
        public Task<Car> UpdateCarSpecification(int carid, string Specification);
        public Task<Car> UpdateCarAvailibility(int carid, bool Availability);
        public Task<Car> UpdateCarDailyRate(int carid, double DailyRate);
        public Task<Car> DeleteCar(int id);
        Task<Car> AddDiscountToCar(int carId, int discountId);
        Task<Car> RemoveDiscountFromCar(int carId, int discountId);
        Task<bool> ViewCarAvailability(int carId);
        Task<List<Reservation>> ViewAllReservations();
        Task<Reservation> ViewReservationDetailsForAdmin(int reservationId);
        Task<List<Car>> GetCarsByAvailabilityStatus();
    }
}
