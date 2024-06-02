using RoadReady.Models;

namespace RoadReady.Interface
{
    public interface IRentalStoreAdminService
    {
        public Task<List<RentalStore>> GetAllRentalStores();
        public Task<List<CarStore>> GetCarsInStore(int storeId);
    }
}
