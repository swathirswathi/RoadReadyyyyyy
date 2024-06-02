using RoadReady.Models;

namespace RoadReady.Interface
{
    public interface IDiscountUserService
    {
        Task<List<Discount>> ViewAvailableDiscounts();
        Task<Reservation> ApplyDiscountToReservation(int reservationId, string discountCode);
        Task<List<Discount>> ViewAppliedDiscounts(int reservationId);
        Task<Discount> ViewDiscountDetails(int discountId);
        Task<List<Car>> ViewDiscountedCars();
    }
}
