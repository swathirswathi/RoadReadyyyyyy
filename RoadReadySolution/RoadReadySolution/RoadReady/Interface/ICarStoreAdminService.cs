using RoadReady.Models;

namespace RoadReady.Interface
{
    public interface ICarStoreAdminService
    {
        Task<CarStore> AddCarToStore(int storeId, int carId);
        Task<CarStore> RemoveCarFromStore(int storeId, int carId);
        Task<List<Car>> ViewAllCarsInAllStores();
       // Task<List<CarStore>> ViewAllCarsInStore(int storeId);
    }
}
