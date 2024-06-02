using System.ComponentModel.DataAnnotations;

namespace RoadReady.Models
{
    public class RentalStore : IEquatable<RentalStore>
    {
        public int StoreId { get; set; }
        [Required(ErrorMessage = "PickUpStoreLocation is Required")]
        public string PickUpStoreLocation{ get; set; }
        [Required(ErrorMessage = "DropOffStoreLocation is Required")]
        public string DropOffStoreLocation{ get; set; }
       
        // Navigation property for the one-to-many relationship
        public ICollection<CarStore>? CarStore { get; set; }

        //Default Constructor 
        public RentalStore()
        {
            StoreId= 0;
        }

        //Parameterized Constructor
        public RentalStore(int storeId, string pickUpStoreLocation, string dropOffStoreLocation)
        {
            StoreId = storeId;
            PickUpStoreLocation = pickUpStoreLocation;
            DropOffStoreLocation = dropOffStoreLocation;
        }
        public RentalStore( string pickUpStoreLocation, string dropOffStoreLocation)
        {
            
            PickUpStoreLocation = pickUpStoreLocation;
            DropOffStoreLocation = dropOffStoreLocation;
        }
        public bool Equals(RentalStore? other)
        {
            var rentalStore = other ?? new RentalStore();
            return this.StoreId.Equals(rentalStore.StoreId);
        }
    }
}
