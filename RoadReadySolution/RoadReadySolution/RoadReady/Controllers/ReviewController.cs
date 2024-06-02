using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using RoadReady.Services;

namespace RoadReady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("ReactPolicy")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewAdminService _reviewAdminService;
        private readonly IReviewUserService _reviewUserService;

        public ReviewController(IReviewAdminService reviewAdminService, IReviewUserService reviewUserService)
        {  
            _reviewAdminService = reviewAdminService;
            _reviewUserService = reviewUserService;
        }

        //Admin Action
        [Authorize(Roles = "user,admin")]
        [HttpGet("Admin/user/GetAllReview")]
        public async Task<ActionResult<List<Review>>> GetAllReviews()
        {
            try
            {
                var allReviews = await _reviewAdminService.GetAllReviews();
                if (allReviews.Any())
                {
                    return Ok(allReviews);
                }
                else
                {
                    return NotFound("No reviews found.");
                }
            }
            catch (NoSuchReviewException ex)
            {
                return NotFound($"No reviews found: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting all reviews: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin,user")]
        [HttpGet("admin/user/car/{carId}")]
        public async Task<ActionResult<List<Review>>> GetCarReviews(int carId)
        {
            try
            {
                var carReviews = await _reviewAdminService.GetCarReviews(carId);
                if (carReviews.Any())
                {
                    return Ok(carReviews);
                }
                else
                {
                    return NotFound($"No reviews found for car with ID {carId}.");
                }
            }
            catch (NoSuchReviewException ex)
            {
                return NotFound($"No reviews found for the car: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting car reviews: {ex.Message}");
            }
        }

        //User Action
        [Authorize(Roles = "user,admin")]
        [HttpPost("user/admin/add")]
        public async Task<ActionResult<Review>> AddReview(Review review)
        {
            try
            {
                var addedReview = await _reviewUserService.AddReview(review);
                if (addedReview != null)
                {
                    return Ok(addedReview);
                }
                else
                {
                    return BadRequest("Failed to add the review.");
                }
            }
            catch (FailedReviewException ex)
            {
                return BadRequest($"Error adding review: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding a review: {ex.Message}");
            }
        }

        [Authorize(Roles = "user,admin")]
        [HttpGet("user/admin/reviewId/{reviewId}")]
        public async Task<ActionResult<Review>> GetReviewDetails(int reviewId)
        {
            try
            {
                var review = await _reviewUserService.GetReviewDetails(reviewId);
                if (review != null)
                {
                    return Ok(review);
                }
                else
                {
                    return NotFound($"Review with ID {reviewId} not found.");
                }
            }
            catch (NoSuchReviewException ex)
            {
                return NotFound($"No such review found: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting review details: {ex.Message}");
            }
        }

        [Authorize(Roles = "user,admin")]
        [HttpGet("user/admin/userId/{userId}")]
        public async Task<ActionResult<List<Review>>> GetUserReviews(int userId)
        {
            try
            {
                var userReviews = await _reviewUserService.GetUserReviews(userId);
                if (userReviews.Any())
                {
                    return Ok(userReviews);
                }
                else
                {
                    return NotFound($"No reviews found for user with ID {userId}.");
                }
            }
            catch (NoSuchReviewException ex)
            {
                return NotFound($"No reviews found for the user: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while getting user reviews: {ex.Message}");
            }
        }

        [Authorize(Roles = "admin,user")]
        [HttpDelete("admin/cars/{id}")]
        public async Task<ActionResult<Review>> DeleteReview(int reviewid)
        {
            try
            {
                var reviews = await _reviewAdminService.DeleteReview(reviewid);
                if (reviews != null)
                {
                    return Ok("Review deleted successfully");
                }
                else
                {
                    return NotFound($"Review with ID {reviewid} not found.");
                }
            }
            catch (NoSuchCarException)
            {
                return NotFound($"Review with ID {reviewid} not found.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the car.");
            }
        }
    }
}
