using RoadReady.Models;

namespace RoadReady.Interface
{
    public interface ICarStoreUserService
    {
        Task<List<CarStore>> ViewAllCarsInStore(int storeId);
    }
}
