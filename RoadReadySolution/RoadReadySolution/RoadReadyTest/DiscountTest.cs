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
    internal class DiscountTest
    {
        CarRentalDbContext _context;
        private DiscountService _discountService;
        private Mock<IRepository<int, Discount>> _mockDiscountRepository;
        private Mock<IRepository<int, Car>> _mockCarRepository;
        private Mock<IRepository<int, Reservation>> _mockReservationRepository;
        private Mock<ILogger<DiscountService>> _mockLogger;

        // <summary>
        /// Setup Method in which InMemory dummy database is created
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CarRentalDbContext>().UseInMemoryDatabase("dummyDatabase").Options;
            _context = new CarRentalDbContext(options);

            _mockDiscountRepository = new Mock<IRepository<int, Discount>>();
            _mockCarRepository = new Mock<IRepository<int, Car>>();
            _mockReservationRepository = new Mock<IRepository<int, Reservation>>();
            _mockLogger = new Mock<ILogger<DiscountService>>();
            _discountService = new DiscountService(_mockDiscountRepository.Object,  _mockReservationRepository.Object,_mockCarRepository.Object,_mockLogger.Object);
        }

        [Test]
        public async Task AddNewDiscount_ValidDiscount_ReturnsDiscount()
        {
            // Arrange
            var validDiscount = new Discount
            {
                // Initialize with valid discount data here for testing
                DiscountId = 1,
                DiscountName = "MegaDiscount",
                CarId = 1,
                StartDateOfDiscount = DateTime.Now,
                EndDateOfDiscount = DateTime.Now.AddDays(7),
                CoupenCode = "DISCOUNT123",
                DiscountPercentage = 10.0
            };

            _mockDiscountRepository.Setup(repo => repo.Add(validDiscount)).ReturnsAsync(validDiscount);

            // Act
            var result = await _discountService.AddNewDiscount(validDiscount);

            // Assert
            Assert.That(result, Is.TypeOf<Discount>());
            Assert.AreEqual(validDiscount, result);
        }
        [Test]
        public async Task AddNewDiscountTest_ValidDiscount_ReturnsAddedDiscount()
        {
            // Arrange
            var discountToAdd = new Discount { DiscountId = 1, DiscountName = "Test Discount" };
            _mockDiscountRepository.Setup(repo => repo.Add(discountToAdd)).ReturnsAsync(discountToAdd);

            // Act
            var addedDiscount = await _discountService.AddNewDiscount(discountToAdd);

            // Assert
            Assert.IsNotNull(addedDiscount);
            Assert.AreEqual(discountToAdd, addedDiscount);
        }



        [Test]
        public async Task AssignDiscountToCar_ValidInputs_ReturnsTrue()
        {
            // Arrange
            var discountId = 1;
            var carId = 2;
            var existingDiscount = new Discount { DiscountId = discountId };
            var car = new Car { CarId = carId };

            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync(existingDiscount);
            _mockCarRepository.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(car);
            _mockCarRepository.Setup(repo => repo.Update(car)).ReturnsAsync(car);

            // Act
            var isAssigned = await _discountService.AssignDiscountToCar(discountId, carId);

            // Assert
            Assert.IsTrue(isAssigned);
        }

        [Test]
        public async Task DeactivateDiscount_ValidDiscountId_ReturnsTrue()
        {
            // Arrange
            var discountId = 1;
            var existingDiscount = new Discount { DiscountId = discountId };

            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync(existingDiscount);
            _mockDiscountRepository.Setup(repo => repo.Update(existingDiscount)).ReturnsAsync(existingDiscount);

            // Act
            var isDeactivated = await _discountService.DeactivateDiscount(discountId);

            // Assert
            Assert.IsTrue(isDeactivated);
        }

        [Test]
        public async Task RemoveDiscountFromCar_ValidInputs_ReturnsTrue()
        {
            // Arrange
            var discountId = 1;
            var carId = 2;
            var existingDiscount = new Discount { DiscountId = discountId };
            var car = new Car { CarId = carId, Discounts = new List<Discount> { existingDiscount } };

            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync(existingDiscount);
            _mockCarRepository.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(car);
            _mockCarRepository.Setup(repo => repo.Update(car)).ReturnsAsync(car);

            // Act
            var isRemoved = await _discountService.RemoveDiscountFromCar(discountId, carId);

            // Assert
            Assert.IsTrue(isRemoved);
        }

        [Test]
        public async Task ViewAllDiscounts_ReturnsListOfDiscounts()
        {
            // Arrange
            var discounts = new List<Discount> { new Discount { DiscountId = 1 }, new Discount { DiscountId = 2 } };
            _mockDiscountRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(discounts);

            // Act
            var allDiscounts = await _discountService.ViewAllDiscounts();

            // Assert
            Assert.IsNotNull(allDiscounts);
            Assert.AreEqual(discounts.Count, allDiscounts.Count);
        }

        [Test]
        public async Task ViewAppliedDiscounts_ValidReservationId_ReturnsListOfDiscounts()
        {
            // Arrange
            var reservationId = 1;

            // Assuming AppliedDiscounts is a collection (List, ICollection, etc.)
            var appliedDiscountsList = new List<Discount> { new Discount { DiscountId = 1 } };

            var reservation = new Reservation { ReservationId = reservationId, AppliedDiscounts = appliedDiscountsList };

            _mockReservationRepository.Setup(repo => repo.GetAsyncById(reservationId)).ReturnsAsync(reservation);

            // Act
            var appliedDiscounts = await _discountService.ViewAppliedDiscounts(reservationId);

            // Assert
            Assert.IsNotNull(appliedDiscounts);
            Assert.AreEqual(1, appliedDiscounts.Count);
        }



        [Test]
        public async Task ViewCarsWithDiscounts_ReturnsListOfCars()
        {
            // Arrange
            var discounts = new List<Discount> { new Discount { DiscountId = 1, Cars = new List<Car> { new Car { CarId = 1 } } } };
            _mockDiscountRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(discounts);

            // Act
            var carsWithDiscounts = await _discountService.ViewCarsWithDiscounts();

            // Assert
            Assert.IsNotNull(carsWithDiscounts);
            Assert.AreEqual(1, carsWithDiscounts.Count);
        }

        [Test]
        public async Task ViewDiscountDetails_ValidDiscountId_ReturnsDiscount()
        {
            // Arrange
            var discountId = 1;
            var discount = new Discount { DiscountId = discountId };

            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync(discount);

            // Act
            var discountDetails = await _discountService.ViewDiscountDetails(discountId);

            // Assert
            Assert.IsNotNull(discountDetails);
            Assert.AreEqual(discountId, discountDetails.DiscountId);
        }

        [Test]
        public async Task ViewDiscountedCars_ReturnsListOfCars()
        {
            // Arrange
            var discounts = new List<Discount> { new Discount { DiscountId = 1, Cars = new List<Car> { new Car { CarId = 1 } } } };
            _mockDiscountRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(discounts);

            // Act
            var discountedCars = await _discountService.ViewDiscountedCars();

            // Assert
            Assert.IsNotNull(discountedCars);
            Assert.AreEqual(1, discountedCars.Count);
        }

        [Test]
        public async Task ViewAvailableDiscounts_ReturnsActiveDiscounts()
        {
            // Arrange
            var currentDate = DateTime.Now;
            var activeDiscounts = new List<Discount>
            {
                new Discount { DiscountId = 1, StartDateOfDiscount = currentDate.AddDays(-1), EndDateOfDiscount = currentDate.AddDays(1) },
                new Discount { DiscountId = 2, StartDateOfDiscount = currentDate.AddDays(-2), EndDateOfDiscount = currentDate.AddDays(2) },
                new Discount { DiscountId = 3, StartDateOfDiscount = currentDate.AddDays(-3), EndDateOfDiscount = currentDate.AddDays(3) }
            };
            var allDiscounts = activeDiscounts.Concat(new List<Discount>
            {
                new Discount { DiscountId = 4, StartDateOfDiscount = currentDate.AddDays(-10), EndDateOfDiscount = currentDate.AddDays(-5) }, // Expired discount
                new Discount { DiscountId = 5, StartDateOfDiscount = currentDate.AddDays(5), EndDateOfDiscount = currentDate.AddDays(10) } // Future discount
            }).ToList();

            _mockDiscountRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(allDiscounts);

            // Act
            var result = await _discountService.ViewAvailableDiscounts();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEquivalent(activeDiscounts, result);
        }

        [Test]
        public void ViewAvailableDiscounts_ExceptionThrown_ThrowsException()
        {
            // Arrange
            _mockDiscountRepository.Setup(repo => repo.GetAsync()).ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _discountService.ViewAvailableDiscounts());
        }

        [Test]
        public async Task ViewDiscountDetails_ReturnsDiscount()
        {
            // Arrange
            var discountId = 1;
            var discount = new Discount { DiscountId = discountId };
            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync(discount);

            // Act
            var result = await _discountService.ViewDiscountDetails(discountId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(discount, result);
        }

        [Test]
        public void ViewDiscountDetails_ExceptionThrown_ThrowsException()
        {
            // Arrange
            var discountId = 1;
            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _discountService.ViewDiscountDetails(discountId));
        }

       
        [Test]
        public void ViewDiscountedCars_ExceptionThrown_ThrowsException()
        {
            // Arrange
            _mockDiscountRepository.Setup(repo => repo.GetAsync()).ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _discountService.ViewDiscountedCars());
        }

        [Test]
        public async Task UpdateDiscountPercentage_ValidInput_ReturnsUpdatedDiscount()
        {
            // Arrange
            const int discountId = 1;
            const double newPercentage = 20.0;
            var existingDiscount = new Discount { DiscountId = discountId, DiscountPercentage = 10.0 };

            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync(existingDiscount);
            _mockDiscountRepository.Setup(repo => repo.Update(existingDiscount)).ReturnsAsync(existingDiscount);

            // Act
            var result = await _discountService.UpdateDiscountPercentage(discountId, newPercentage);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(discountId, result.DiscountId);
            Assert.AreEqual(newPercentage, result.DiscountPercentage);
        }

        [Test]
        public void UpdateDiscountPercentage_InvalidPercentage_ThrowsArgumentException()
        {
            // Arrange
            const int discountId = 1;
            const double invalidPercentage = -10.0;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _discountService.UpdateDiscountPercentage(discountId, invalidPercentage));
        }

        [Test]
        public void UpdateDiscountPercentage_DiscountNotFound_ThrowsNoSuchDiscountException()
        {
            // Arrange
            const int discountId = 1;
            const double validPercentage = 20.0;

            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync((Discount)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchDiscountException>(async () => await _discountService.UpdateDiscountPercentage(discountId, validPercentage));
        }

        [Test]
        public async Task UpdateDiscountEndDate_ValidInput_ReturnsUpdatedDiscount()
        {
            // Arrange
            int discountId = 1;
            DateTime newEndDate = DateTime.Now.AddDays(10);
            var existingDiscount = new Discount { DiscountId = discountId, EndDateOfDiscount = DateTime.Now.AddDays(5) };

            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync(existingDiscount);
            _mockDiscountRepository.Setup(repo => repo.Update(It.IsAny<Discount>())).ReturnsAsync(existingDiscount);

            // Act
            var result = await _discountService.UpdateDiscountEndDate(discountId, newEndDate);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(newEndDate, result.EndDateOfDiscount);
        }

        [Test]
        public void UpdateDiscountEndDate_EndDateInPast_ThrowsArgumentException()
        {
            // Arrange
            int discountId = 1;
            DateTime pastEndDate = DateTime.Now.AddDays(-1);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _discountService.UpdateDiscountEndDate(discountId, pastEndDate));
        }

        [Test]
        public void UpdateDiscountEndDate_DiscountNotFound_ThrowsNoSuchDiscountException()
        {
            // Arrange
            int discountId = 1;
            DateTime newEndDate = DateTime.Now.AddDays(10);

            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync((Discount)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchDiscountException>(async () => await _discountService.UpdateDiscountEndDate(discountId, newEndDate));
        }

        [Test]
        public async Task UpdateDiscountEndDate1_ValidInput_ReturnsUpdatedDiscount()
        {
            // Arrange
            int discountId = 1;
            DateTime newEndDate = DateTime.Now.AddDays(10);
            var existingDiscount = new Discount { DiscountId = discountId, EndDateOfDiscount = DateTime.Now.AddDays(5) };

            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync(existingDiscount);
            _mockDiscountRepository.Setup(repo => repo.Update(It.IsAny<Discount>())).ReturnsAsync(existingDiscount);

            // Act
            var result = await _discountService.UpdateDiscountEndDate(discountId, newEndDate);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(newEndDate, result.EndDateOfDiscount);
        }

        [Test]
        public void UpdateDiscountEndDate1_EndDateInPast_ThrowsArgumentException()
        {
            // Arrange
            int discountId = 1;
            DateTime pastEndDate = DateTime.Now.AddDays(-1);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _discountService.UpdateDiscountEndDate(discountId, pastEndDate));
        }

        [Test]
        public void UpdateDiscountEndDate1_DiscountNotFound_ThrowsNoSuchDiscountException()
        {
            // Arrange
            int discountId = 1;
            DateTime newEndDate = DateTime.Now.AddDays(10);

            _mockDiscountRepository.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync((Discount)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchDiscountException>(async () => await _discountService.UpdateDiscountEndDate(discountId, newEndDate));
        }


    }
}
