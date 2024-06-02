using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RoadReady.Contexts;
using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using RoadReady.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadReadyTest
{
    internal class ReviewTest
    {
        CarRentalDbContext context;
        private ReviewService _reviewService;
        private Mock<IRepository<int, Review>> _mockRepo;
        private Mock<ILogger<ReviewService>> _mockLogger;

        // <summary>
        /// Setup Method in which InMemory dummy database is created
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CarRentalDbContext>().UseInMemoryDatabase("dummyDatabase").Options;
            context = new CarRentalDbContext(options);

            _mockRepo = new Mock<IRepository<int, Review>>();
            //_mockDiscountRepo = new Mock<IRepository<int, Discount>>();
            _mockLogger = new Mock<ILogger<ReviewService>>();
            _reviewService = new ReviewService(_mockRepo.Object, _mockLogger.Object);

        }

        [Test]
        public async Task AddReviewTest_ValidReview_ReturnsAddedReview()
        {
            // Arrange

            _mockRepo.Setup(repo => repo.Add(It.IsAny<Review>()))
                          .ReturnsAsync((Review addedReview) => addedReview);


            var newReview = new Review
            {
                ReviewId = 1,
                Rating = 5,
                Comments = "Great car!",
                ReviewDate = DateTime.Today,
                UserId = 1,
                CarId = 1
            };

            // Act
            var result = await _reviewService.AddReview(newReview);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(newReview.ReviewId, result.ReviewId);
            Assert.AreEqual(newReview.Rating, result.Rating);
            Assert.AreEqual(newReview.Comments, result.Comments);
            Assert.AreEqual(newReview.ReviewDate, result.ReviewDate);
            Assert.AreEqual(newReview.UserId, result.UserId);
            Assert.AreEqual(newReview.CarId, result.CarId);
        }

        [Test]
        public async Task GetAllReviewsTest_NoReviewsFound_ThrowsNoSuchReviewException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(new List<Review>());

            // Act & Assert
            Assert.ThrowsAsync<NoSuchReviewException>(async () => await _reviewService.GetAllReviews());
        }

        [Test]
        public async Task GetCarReviewsTest_CarHasReviews_ReturnsListOfReviews()
        {
            // Arrange
            int carId = 1;
            var carReviews = new List<Review>
{
    new Review { ReviewId = 1, CarId = carId, Rating = 4, Comments = "Good car" },
    new Review { ReviewId = 2, CarId = carId, Rating = 5, Comments = "Excellent car" }
};

            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(carReviews);

            // Act
            var result = await _reviewService.GetCarReviews(carId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(carReviews.Count, result.Count);
            Assert.AreEqual(carReviews[0].ReviewId, result[0].ReviewId);
            // Add similar assertions for other properties
        }

        [Test]
        public async Task GetCarReviewsTest_NoReviewsForCar_ThrowsNoSuchReviewException()
        {
            // Arrange
            int carId = 1;

            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(new List<Review>());

            // Act & Assert
            Assert.ThrowsAsync<NoSuchReviewException>(async () => await _reviewService.GetCarReviews(carId));
        }

        [Test]
        public async Task GetReviewDetailsTest_ReviewExists_ReturnsReview()
        {
            // Arrange
            int reviewId = 1;
            var review = new Review { ReviewId = reviewId, Rating = 5, Comments = "Great review" };

            _mockRepo.Setup(repo => repo.GetAsyncById(reviewId))
                     .ReturnsAsync(review);

            // Act
            var result = await _reviewService.GetReviewDetails(reviewId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(reviewId, result.ReviewId);
            // Add similar assertions for other properties
        }

        [Test]
        public async Task GetReviewDetails_ReviewNotFoundTest_ThrowsNoSuchReviewException()
        {
            // Arrange
            int reviewId = 1;

            _mockRepo.Setup(repo => repo.GetAsyncById(reviewId))
                     .ReturnsAsync((Review)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchReviewException>(async () => await _reviewService.GetReviewDetails(reviewId));
        }

        [Test]
        public async Task GetUserReviewsTest_UserHasReviews_ReturnsListOfReviews()
        {
            // Arrange
            int userId = 1;
            var userReviews = new List<Review>
{
    new Review { ReviewId = 1, UserId = userId, Rating = 4, Comments = "Good review" },
    new Review { ReviewId = 2, UserId = userId, Rating = 5, Comments = "Excellent review" }
};

            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(userReviews);

            // Act
            var result = await _reviewService.GetUserReviews(userId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(userReviews.Count, result.Count);
            Assert.AreEqual(userReviews[0].ReviewId, result[0].ReviewId);
            // Add similar assertions for other properties
        }

        [Test]
        public async Task GetUserReviews_NoReviewsForUserTest_ThrowsNoSuchReviewException()
        {
            // Arrange
            int userId = 1;

            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(new List<Review>());

            // Act & Assert
            Assert.ThrowsAsync<NoSuchReviewException>(async () => await _reviewService.GetUserReviews(userId));
        }

        [Test]
        public async Task AddReview_ValidReview_ReturnsAddedReview()
        {
            // Arrange
            var newReview = new Review
            {
                ReviewId = 1,
                Rating = 5,
                Comments = "Great car!",
                ReviewDate = DateTime.Today,
                UserId = 1,
                CarId = 1
            };

            _mockRepo.Setup(repo => repo.Add(It.IsAny<Review>()))
                     .ReturnsAsync(newReview);

            // Act
            var result = await _reviewService.AddReview(newReview);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(newReview.ReviewId, result.ReviewId);
            // Add similar assertions for other properties
        }

        [Test]
        public void AddReview_RepositoryReturnsNull_ThrowsFailedReviewException()
        {
            // Arrange
            var newReview = new Review
            {
                ReviewId = 1,
                Rating = 5,
                Comments = "Great car!",
                ReviewDate = DateTime.Today,
                UserId = 1,
                CarId = 1
            };

            _mockRepo.Setup(repo => repo.Add(It.IsAny<Review>()))
                     .ReturnsAsync((Review)null); // Simulating repository returning null

            // Act & Assert
            var ex = Assert.ThrowsAsync<FailedReviewException>(() => _reviewService.AddReview(newReview));
            Assert.AreEqual("Failed to add review ", ex.Message);
        }

        [Test]
        public void AddReview_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            var newReview = new Review
            {
                ReviewId = 1,
                Rating = 5,
                Comments = "Great car!",
                ReviewDate = DateTime.Today,
                UserId = 1,
                CarId = 1
            };

            _mockRepo.Setup(repo => repo.Add(It.IsAny<Review>()))
                     .ThrowsAsync(new Exception("Simulated repository exception")); // Simulating repository throwing an exception

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _reviewService.AddReview(newReview));
        }

        [Test]
        public async Task DeleteReview_ReviewExists_ReturnsDeletedReview()
        {
            // Arrange
            int reviewId = 1;
            var reviewToDelete = new Review { ReviewId = reviewId };

            _mockRepo.Setup(repo => repo.Delete(reviewId))
                     .ReturnsAsync(reviewToDelete);

            // Act
            var result = await _reviewService.DeleteReview(reviewId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(reviewId, result.ReviewId);
        }

        [Test]
        public void DeleteReview_ReviewNotFound_ThrowsNoSuchCarException()
        {
            // Arrange
            int reviewId = 1;

            _mockRepo.Setup(repo => repo.Delete(reviewId))
                     .ThrowsAsync(new NoSuchCarException()); // Simulating review not found exception

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCarException>(() => _reviewService.DeleteReview(reviewId));
        }

        [Test]
        public void DeleteReview_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            int reviewId = 1;

            _mockRepo.Setup(repo => repo.Delete(reviewId))
                     .ThrowsAsync(new Exception("Simulated repository exception")); // Simulating repository throwing an exception

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _reviewService.DeleteReview(reviewId));
        }

    }
}
