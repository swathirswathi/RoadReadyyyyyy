using RoadReady.Models;

namespace RoadReady.Interface
{
    public interface IDiscountAdminService : IDiscountUserService
    {
        Task<Discount> AddNewDiscount(Discount discount);
        Task<Discount> UpdateDiscountEndDate(int discountId, DateTime endDate);
        Task<Discount> UpdateDiscountPercentage(int discountId, double percentage);
        Task<bool> DeactivateDiscount(int discountId);
        Task<List<Discount>> ViewAllDiscounts();
        Task<bool> AssignDiscountToCar(int discountId, int carId);
        Task<List<Discount>> ViewCarsWithDiscounts();
        Task<bool> RemoveDiscountFromCar(int discountId, int carId);
    }
}
