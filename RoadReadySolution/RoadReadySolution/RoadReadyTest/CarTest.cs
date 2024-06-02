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
    internal class CarTest
    {
        CarRentalDbContext _context;
        private CarService _carService;
        private Mock<IRepository<int, Car>> _mockRepo;
        private Mock<IRepository<int, Discount>> _mockDiscountRepo;
        private Mock<ILogger<CarService>> _mockLogger;

        // <summary>
        /// Setup Method in which InMemory dummy database is created
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CarRentalDbContext>().UseInMemoryDatabase("dummyDatabase").Options;
            _context = new CarRentalDbContext(options);

            _mockRepo = new Mock<IRepository<int, Car>>();
            _mockDiscountRepo=new Mock<IRepository<int, Discount>>();
            _mockLogger = new Mock<ILogger<CarService>>();
            _carService = new CarService(_mockRepo.Object, _mockDiscountRepo.Object, _mockLogger.Object);
        }


        [Test]
        [Order(1)]
        public async Task AddCarTest_ReturnsCar()
        {
            // Arrange
            var validCar = new Car
            {
                CarId = 1,
                Make = "Toyota",
                Model = "Camry",
                Year = 2022,
                Availability = true,
                DailyRate = 50.0,
                ImageURL = " ",
                Specification = "Automatic transmission, 4 doors, 5 seats"
                // Populate other properties as needed
            };
            _mockRepo.Setup(repo => repo.Add(validCar)).ReturnsAsync(validCar);

            // Act
            var result = await _carService.AddCar(validCar);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Car>(result);
            Assert.AreEqual(validCar, result);
        }

        [Test]
        [Order(2)]
        public async Task AddCar_WithExistingCarId_ThrowsCarAlreadyExistsException()
        {
            // Arrange
            var existingCarId = 1;
            var existingCar = new Car { CarId = existingCarId };
            _mockRepo.Setup(repo => repo.Add(existingCar)).ThrowsAsync(new CarAlreadyExistsException());

            // Act & Assert
            Assert.That(async () => await _carService.AddCar(existingCar), Throws.TypeOf<CarAlreadyExistsException>());
        }

        [Test]
        public async Task AddDiscountToCar_ValidIds_ReturnsCarWithDiscount()
        {
            // Arrange
            int carId = 1;
            int discountId = 1;
            var existingDiscount = new Discount { DiscountId = discountId, /* Add other properties as needed */ };
            var existingCar = new Car { CarId = carId, /* Add other properties as needed */ };

            _mockDiscountRepo.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync(existingDiscount);
            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(existingCar);
            _mockRepo.Setup(repo => repo.Update(It.IsAny<Car>())).ReturnsAsync(existingCar);

            // Act
            var result = await _carService.AddDiscountToCar(carId, discountId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Discounts, Contains.Item(existingDiscount));

        }
        [Test]
        public async Task AddDiscountToCar_DiscountNotFound_ReturnsNull()
        {
            // Arrange
            int carId = 1;
            int discountId = 1;

            _mockDiscountRepo.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync((Discount)null);
            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(new Car());

            // Act
            var result = await _carService.AddDiscountToCar(carId, discountId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddDiscountToCar_CarNotFound_ReturnsNull()
        {
            // Arrange
            int carId = 1;
            int discountId = 1;

            _mockDiscountRepo.Setup(repo => repo.GetAsyncById(discountId)).ReturnsAsync(new Discount());
            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync((Car)null);

            // Act
            var result = await _carService.AddDiscountToCar(carId, discountId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddDiscountToCar_ExceptionThrown_ReturnsNull()
        {
            // Arrange
            int carId = 1;
            int discountId = 1;

            _mockDiscountRepo.Setup(repo => repo.GetAsyncById(discountId)).ThrowsAsync(new Exception());
            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(new Car());

            // Act
            var result = await _carService.AddDiscountToCar(carId, discountId);

            // Assert
            Assert.IsNull(result);
        }
        [Test]
        public async Task DeleteCar_ExistingCar_DeletesCar()
        {
            // Arrange
            int carId = 1;

            _mockRepo.Setup(repo => repo.Delete(carId)).ReturnsAsync(new Car { CarId = carId });

            // Act
            var result = await _carService.DeleteCar(carId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(carId, result.CarId);
        }
        [Test]
        public async Task GetCarById_ExistingCar_ReturnsCar()
        {
            // Arrange
            int existingCarId = 1;
            Car existingCar = new Car { CarId = existingCarId, Make = "Toyota", Model = "Camry", Year = 2022 };

            _mockRepo.Setup(repo => repo.GetAsyncById(existingCarId)).ReturnsAsync(existingCar);

            // Act
            var result = await _carService.GetCarById(existingCarId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(existingCarId, result.CarId);
            Assert.AreEqual(existingCar.Make, result.Make);
            Assert.AreEqual(existingCar.Model, result.Model);
            Assert.AreEqual(existingCar.Year, result.Year);
        }

        [Test]
        public async Task GetCarsByAvailabilityStatus_ReturnsAvailableCars()
        {
            // Arrange
            var allCars = new List<Car>
            {
             new Car { CarId = 1, Make = "Toyota", Model = "Camry", Year = 2020, Availability = true },
             new Car { CarId = 2, Make = "Honda", Model = "Accord", Year = 2019, Availability = false },
             new Car { CarId = 3, Make = "Ford", Model = "Fusion", Year = 2018, Availability = true }
        
             };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(allCars);

            // Act
            var result = await _carService.GetCarsByAvailabilityStatus();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(2)); // Assuming two cars are available
            Assert.That(result, Is.All.Matches<Car>(c => c.Availability == true));// All cars should be available
        }

        [Test]
        public async Task GetCarsList_ReturnsListOfCars()
        {
            // Arrange
            var cars = new List<Car>
        {
        // Populate with sample cars for testing
        new Car { CarId = 1, Make = "Toyota", Model = "Camry", Year = 2020, Availability = true },
        new Car { CarId = 2, Make = "Honda", Model = "Civic", Year = 2019, Availability = true },
        
        };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(cars);

            // Act
            var result = await _carService.GetCarsList();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(cars.Count));
            Assert.That(result, Is.EqualTo(cars));
        }
        [Test]
        public async Task MakeReservation_ValidInputs_ReturnsReservation()
        {
            // Arrange
            int carId = 1;
            DateTime startDate = DateTime.Now.AddDays(1);
            DateTime endDate = DateTime.Now.AddDays(3);

            var car = new Car
            {
                CarId = carId,
                Availability = true,
                Reservations = new List<Reservation>()
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(car);

            // Act
            var result = await _carService.MakeReservation(carId, startDate, endDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(carId, result.CarId);
            Assert.AreEqual(startDate, result.PickUpDateTime);
            Assert.AreEqual(endDate, result.DropOffDateTime);
        }

        [Test]
        public async Task MakeReservation_CarNotAvailable_ThrowsNoSuchCarException()
        {
            // Arrange
            int carId = 1;
            DateTime startDate = DateTime.Now.AddDays(1);
            DateTime endDate = DateTime.Now.AddDays(3);

            var car = new Car
            {
                CarId = carId,
                Availability = false
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(car);

            // Act & Assert
            var exception = Assert.ThrowsAsync<NoSuchCarException>(() => _carService.MakeReservation(carId, startDate, endDate));
            Assert.IsNotNull(exception);
        }

        [Test]
        public async Task RemoveDiscountFromCar_ValidInputs_ReturnsCar()
        {
            // Arrange
            int carId = 1;
            int discountId = 1;

            var car = new Car
            {
                CarId = carId,
                Discounts = new List<Discount>
        {
            new Discount { DiscountId = discountId }
        }
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(car);

            // Act
            var result = await _carService.RemoveDiscountFromCar(carId, discountId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(carId, result.CarId);
            Assert.IsFalse(result.Discounts.Any(d => d.DiscountId == discountId));
        }

        [Test]
        public async Task RemoveDiscountFromCar_DiscountNotAssigned_ThrowsDiscountNotAssignedToCarException()
        {
            // Arrange
            int carId = 1;
            int discountId = 1;

            var car = new Car
            {
                CarId = carId,
                Discounts = new List<Discount>()
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(car);

            // Act & Assert
            var exception = Assert.ThrowsAsync<DiscountNotAssignedToCarException>(() => _carService.RemoveDiscountFromCar(carId, discountId));
            Assert.IsNotNull(exception); ;
        }
        [Test]
        public async Task UpdateCarDailyRate_ExistingCar_ReturnsUpdatedCar()
        {
            // Arrange
            int carId = 1;
            double newDailyRate = 50.00;
            var existingCar = new Car { CarId = carId, DailyRate = 30.00 };
            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(existingCar);
            _mockRepo.Setup(repo => repo.Update(existingCar)).ReturnsAsync(existingCar);

            // Act
            var result = await _carService.UpdateCarDailyRate(carId, newDailyRate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(newDailyRate, result.DailyRate);
        }

        [Test]
        public async Task UpdateCarSpecification_ExistingCar_ReturnsUpdatedCar()
        {
            // Arrange
            int carId = 1;
            string newSpecification = "Updated specification";
            var existingCar = new Car { CarId = carId, Specification = "Original specification" };
            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(existingCar);
            _mockRepo.Setup(repo => repo.Update(existingCar)).ReturnsAsync(existingCar);

            // Act
            var result = await _carService.UpdateCarSpecification(carId, newSpecification);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(newSpecification, result.Specification);
        }
        [Test]
        public async Task ViewAllReservations_ReturnsListOfReservations()
        {
            // Arrange
            var car1 = new Car { CarId = 1, Reservations = new List<Reservation> { new Reservation { ReservationId = 1 } } };
            var car2 = new Car { CarId = 2, Reservations = new List<Reservation> { new Reservation { ReservationId = 2 } } };
            var cars = new List<Car> { car1, car2 };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(cars);

            // Act
            var result = await _carService.ViewAllReservations();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result, Has.Some.Property("ReservationId").EqualTo(1));
            Assert.That(result, Has.Some.Property("ReservationId").EqualTo(2));
        }

        [Test]
        public async Task ViewCarDetails_ReturnsCarDetails()
        {
            // Arrange
            var carId = 1;
            var expectedCar = new Car { CarId = carId, Make = "Toyota", Model = "Camry", Year = 2022 };

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(expectedCar);

            // Act
            var result = await _carService.ViewCarDetails(carId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCar));
        }
        [Test]
        public async Task ViewPastReservations_ReturnsPastReservationsForUser()
        {
            // Arrange
            var userId = 1;
            var currentDateTime = DateTime.Now;
            var pastReservations = new List<Reservation>
         {
        new Reservation { UserId = userId, DropOffDateTime = currentDateTime.AddDays(-1) },
        new Reservation { UserId = userId, DropOffDateTime = currentDateTime.AddDays(-2) },
        // Add more past reservations as needed...
         };

            var allCars = new List<Car>
        {
        new Car { Reservations = pastReservations },
        // Add more cars with reservations as needed...
        };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(allCars);

            // Act
            var result = await _carService.ViewPastReservations(userId);

            // Assert
            Assert.That(result, Is.EqualTo(pastReservations));
        }

        [Test]
        public async Task ViewPayments_ReturnsPaymentsForUser()
        {
            // Arrange
            var userId = 1;
            var payments = new List<Payment>
         {
        new Payment { UserId = userId },
       
         };

            var allCars = new List<Car>
         {
        new Car { Payments = payments },
       
           };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(allCars);

            // Act
            var result = await _carService.ViewPayments(userId);

            // Assert
            Assert.That(result, Is.EqualTo(payments));
        }

        [Test]
        public async Task ViewReservationDetails_ReturnsReservationDetails()
        {
            // Arrange
            var reservationId = 1;
            var reservation = new Reservation { ReservationId = reservationId };
            var allCars = new List<Car>
        {
        new Car { Reservations = new List<Reservation> { reservation } },
       
        };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(allCars);

            // Act
            var result = await _carService.ViewReservationDetails(reservationId);

            // Assert
            Assert.That(result, Is.EqualTo(reservation));
        }

        [Test]
        public async Task ViewReservationDetailsForAdmin_ReturnsReservationDetails()
        {
            // Arrange
            var reservationId = 1;
            var reservation = new Reservation { ReservationId = reservationId };
            var allCars = new List<Car>
         {
        new Car { Reservations = new List<Reservation> { reservation } },
        
          };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(allCars);

            // Act
            var result = await _carService.ViewReservationDetailsForAdmin(reservationId);

            // Assert
            Assert.That(result, Is.EqualTo(reservation));
        }
        [Test]
        public async Task ViewReviews_ReturnsReviewsForCar()
        {
            // Arrange
            var carId = 1;
            var reviews = new List<Review>
           {
        new Review { CarId = carId },
       
            };
            var car = new Car { CarId = carId, Reviews = reviews };

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(car);

            // Act
            var result = await _carService.ViewReviews(carId);

            // Assert
            Assert.That(result, Is.EqualTo(reviews));
        }

        [Test]
        public async Task UpdateCarAvailibility_ExistingCar_ReturnsUpdatedCar()
        {
            // Arrange
            int carId = 1;
            bool newAvailability = false;
            var existingCar = new Car { CarId = carId, Availability = true };

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(existingCar);
            _mockRepo.Setup(repo => repo.Update(existingCar)).ReturnsAsync(existingCar);

            // Act
            var result = await _carService.UpdateCarAvailibility(carId, newAvailability);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(newAvailability, result.Availability);
        }

        [Test]
        public async Task UpdateCarAvailibility_NonExistingCar_ReturnsNull()
        {
            // Arrange
            int carId = 1;
            bool newAvailability = false;

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync((Car)null);

            // Act
            var result = await _carService.UpdateCarAvailibility(carId, newAvailability);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UpdateCarAvailibility_ExceptionThrown_ThrowsException()
        {
            // Arrange
            int carId = 1;
            bool newAvailability = false;

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _carService.UpdateCarAvailibility(carId, newAvailability));
        }


        [Test]
        public async Task UpdateCarDailyRate_NonExistingCar_ReturnsNull()
        {
            // Arrange
            int carId = 1;
            double newDailyRate = 50.00;

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync((Car)null);

            // Act
            var result = await _carService.UpdateCarDailyRate(carId, newDailyRate);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UpdateCarDailyRate_ExceptionThrown_ThrowsException()
        {
            // Arrange
            int carId = 1;
            double newDailyRate = 50.00;

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _carService.UpdateCarDailyRate(carId, newDailyRate));
        }

        [Test]
        public async Task UpdateCarSpecification_NonExistingCar_ReturnsNull()
        {
            // Arrange
            int carId = 1;
            string newSpecification = "Updated specification";

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync((Car)null);

            // Act
            var result = await _carService.UpdateCarSpecification(carId, newSpecification);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UpdateCarSpecification_ExceptionThrown_ThrowsException()
        {
            // Arrange
            int carId = 1;
            string newSpecification = "Updated specification";

            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _carService.UpdateCarSpecification(carId, newSpecification));
        }

        [Test]
        public async Task ViewAvailableCars_ExceptionThrown_ThrowsException()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(1);
            var endDate = DateTime.Now.AddDays(3);

            _mockRepo.Setup(repo => repo.GetAsync()).ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _carService.ViewAvailableCars(startDate, endDate));
        }

        

        [Test]
        public async Task ViewCarAvailability_ReturnsFalse_WhenCarIsNotAvailable()
        {
            // Arrange
            var carId = 2;
            var unavailableCar = new Car { CarId = carId, Availability = false };
            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(unavailableCar);

            // Act
            var result = await _carService.ViewCarAvailability(carId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ViewCarAvailability_ThrowsNoSuchCarException_WhenCarNotFound()
        {
            // Arrange
            var carId = 3;
            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync((Car)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCarException>(() => _carService.ViewCarAvailability(carId));
        }


        [Test]
        public void ViewCarDetails_ThrowsNoSuchCarException_WhenCarNotFound()
        {
            // Arrange
            var carId = 2;
            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync((Car)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCarException>(() => _carService.ViewCarDetails(carId));
        }

        [Test]
        public async Task ViewAvailableCars_ReturnsAvailableCars()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(1);
            var endDate = DateTime.Now.AddDays(3);
            var availableCar = new Car { CarId = 1, Availability = true };
            var unavailableCar = new Car { CarId = 2, Availability = false };
            var allCars = new List<Car> { availableCar, unavailableCar };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(allCars);

            // Act
            var result = await _carService.ViewAvailableCars(startDate, endDate);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result, Has.Exactly(1).EqualTo(availableCar));
        }

        [Test]
        public void ViewAvailableCars_ThrowsException_WhenRepositoryThrowsException()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(1);
            var endDate = DateTime.Now.AddDays(3);

            _mockRepo.Setup(repo => repo.GetAsync()).ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _carService.ViewAvailableCars(startDate, endDate));
        }

        [Test]
        public async Task ViewCarAvailability_ReturnsTrue_WhenCarIsAvailable()
        {
            // Arrange
            var carId = 1;
            var availableCar = new Car { CarId = carId, Availability = true };
            _mockRepo.Setup(repo => repo.GetAsyncById(carId)).ReturnsAsync(availableCar);

            // Act
            var result = await _carService.ViewCarAvailability(carId);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
