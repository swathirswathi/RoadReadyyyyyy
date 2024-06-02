using RoadReady.Models;

namespace RoadReady.Interface
{
    public interface IReviewAdminService
    {
        Task<List<Review>> GetAllReviews();
        Task<List<Review>> GetCarReviews(int carId);
        Task<Review> DeleteReview(int reviewid);
    }
}
