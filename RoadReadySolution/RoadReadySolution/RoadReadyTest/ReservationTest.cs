﻿using Microsoft.EntityFrameworkCore;
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
    internal class ReservationTest
    {
        private CarRentalDbContext context;
        private Mock<IRepository<int, Reservation>> _mockRepo;
        private Mock<ILogger<ReservationService>> _mockLogger;
        private ReservationService _reservationService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CarRentalDbContext>()
                .UseInMemoryDatabase("dummyDatabase")
                .Options;

            context = new CarRentalDbContext(options);

            _mockRepo = new Mock<IRepository<int, Reservation>>();
            _mockLogger = new Mock<ILogger<ReservationService>>();
            _reservationService = new ReservationService(_mockRepo.Object, _mockLogger.Object);
        }

        [Test]
        public async Task CancelReservationTest_ValidReservation_ReturnsTrue()
        {
            // Arrange
            int reservationId = 1;
            var reservation = new Reservation { ReservationId = reservationId, Status = "Pending" };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            _mockRepo.Setup(repo => repo.Update(reservation))
                     .ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.CancelReservation(reservationId);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("Cancelled", reservation.Status);
        }

        [Test]
        public async Task GetPendingReservationsTest_NoPendingReservations_ThrowsNoPendingReservationsException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(new List<Reservation>());

            // Act & Assert
            Assert.ThrowsAsync<NoPendingReservationsException>(() => _reservationService.GetPendingReservations());
        }

        [Test]
        public async Task UpdateReservationPriceTest_ValidReservation_ReturnsUpdatedReservation()
        {
            // Arrange
            int reservationId = 1;
            double newPrice = 150.0;
            var reservation = new Reservation { ReservationId = reservationId, TotalPrice = 100.0 };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            _mockRepo.Setup(repo => repo.Update(reservation))
                     .ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.UpdateReservationPrice(reservationId, newPrice);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(newPrice, result.TotalPrice);
        }

        [Test]
        public async Task GetUserReservationsTest_UserHasReservations_ReturnsListOfReservations()
        {
            // Arrange
            int userId = 1;
            var userReservations = new List<Reservation>
    {
        new Reservation { ReservationId = 1, UserId = userId, Status = "Pending" },
        new Reservation { ReservationId = 2, UserId = userId, Status = "Confirmed" }
    };

            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(userReservations);

            // Act
            var result = await _reservationService.GetUserReservations(userId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(userReservations.Count, result.Count);
            Assert.AreEqual(userReservations[0].ReservationId, result[0].ReservationId);

        }

        [Test]
        public async Task CreateReservation_ValidReservation_ReturnsCreatedReservation()
        {
            // Arrange
            var newReservation = new Reservation
            {
                ReservationId = 1,
                PickUpDateTime = DateTime.Now,
                DropOffDateTime = DateTime.Now.AddDays(5),
                Status = "Pending",
                PickUpStoreLocation = "Location A",
                DropOffStoreLocation = "Location B",
                TotalPrice = 100.0,
                UserId = 1,
                CarId = 1
            };

            _mockRepo.Setup(repo => repo.Add(It.IsAny<Reservation>()))
                     .ReturnsAsync(newReservation);

            // Act
            var result = await _reservationService.CreateReservation(newReservation);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(newReservation.ReservationId, result.ReservationId);

        }

        [Test]
        public async Task GetAllReservations_ReservationsExist_ReturnsListOfReservations()
        {
            // Arrange
            var reservations = new List<Reservation>
{
    new Reservation { ReservationId = 1, Status = "Pending" },
    new Reservation { ReservationId = 2, Status = "Confirmed" }
};

            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(reservations);

            // Act
            var result = await _reservationService.GetAllReservations();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(reservations.Count, result.Count);
            Assert.AreEqual(reservations[0].ReservationId, result[0].ReservationId);

        }

        [Test]
        public async Task GetPendingReservations_PendingReservationsExist_ReturnsListOfPendingReservations()
        {
            // Arrange
            var reservations = new List<Reservation>
{
    new Reservation { ReservationId = 1, Status = "Pending" },
    new Reservation { ReservationId = 2, Status = "Confirmed" },
    new Reservation { ReservationId = 3, Status = "Pending" }
};

            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(reservations);

            // Act
            var result = await _reservationService.GetPendingReservations();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetReservationDetails_ReservationExists_ReturnsReservation()
        {
            // Arrange
            int reservationId = 1;
            var reservation = new Reservation { ReservationId = reservationId, Status = "Pending" };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.GetReservationDetails(reservationId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(reservationId, result.ReservationId);

        }

        [Test]
        public async Task GetReservsationStatus_ReservationExists_ReturnsUpdatedReservation()
        {
            // Arrange
            int reservationId = 1;
            var reservation = new Reservation { ReservationId = reservationId, Status = "Reserved" };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            _mockRepo.Setup(repo => repo.Update(reservation))
                     .ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.GetReservsationStatus(reservationId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(reservationId, result.ReservationId);
            Assert.AreEqual("Reserved", result.Status);
        }

        [Test]
        public async Task UpdateReservationStatus_ReservationExists_ReturnsUpdatedReservation()
        {
            // Arrange
            int reservationId = 1;
            string newStatus = "Confirmed";
            var reservation = new Reservation { ReservationId = reservationId, Status = "Pending" };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            _mockRepo.Setup(repo => repo.Update(reservation))
                     .ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.UpdateReservationStatus(reservationId, newStatus);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(reservationId, result.ReservationId);
            Assert.AreEqual(newStatus, result.Status); // Status should be updated

        }
        [Test]
        public async Task CancelReservation_ValidReservation_ReturnsTrue()
        {
            // Arrange
            int reservationId = 1;
            var reservation = new Reservation { ReservationId = reservationId, Status = "Pending" };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            _mockRepo.Setup(repo => repo.Update(reservation))
                     .ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.CancelReservation(reservationId);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("Cancelled", reservation.Status);
        }

        [Test]
        public async Task GetPendingReservations_NoPendingReservations_ThrowsNoPendingReservationsException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(new List<Reservation>());

            // Act & Assert
            Assert.ThrowsAsync<NoPendingReservationsException>(() => _reservationService.GetPendingReservations());
        }

        [Test]
        public async Task UpdateReservationPrice_ValidReservation_ReturnsUpdatedReservation()
        {
            // Arrange
            int reservationId = 1;
            double newPrice = 150.0;
            var reservation = new Reservation { ReservationId = reservationId, TotalPrice = 100.0 };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            _mockRepo.Setup(repo => repo.Update(reservation))
                     .ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.UpdateReservationPrice(reservationId, newPrice);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(newPrice, result.TotalPrice);
        }

        [Test]
        public async Task GetUserReservations_UserHasReservations_ReturnsListOfReservations()
        {
            // Arrange
            int userId = 1;
            var userReservations = new List<Reservation>
    {
        new Reservation { ReservationId = 1, UserId = userId, Status = "Pending" },
        new Reservation { ReservationId = 2, UserId = userId, Status = "Confirmed" }
    };

            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(userReservations);

            // Act
            var result = await _reservationService.GetUserReservations(userId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(userReservations.Count, result.Count);
            Assert.AreEqual(userReservations[0].ReservationId, result[0].ReservationId);
        }

        [Test]
        public async Task GetReservationsByCarId_ReservationsExist_ReturnsListOfReservations()
        {
            // Arrange
            int carId = 1;
            var reservations = new List<Reservation>
    {
        new Reservation { ReservationId = 1, CarId = carId, Status = "Pending" },
        new Reservation { ReservationId = 2, CarId = carId, Status = "Confirmed" }
    };

            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(reservations);

            // Act
            var result = await _reservationService.GetReservationsByCarId(carId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(r => r.CarId == carId));
        }

        [Test]
        public void GetReservationsByCarId_NoReservations_ThrowsNoSuchReservationException()
        {
            // Arrange
            int carId = 1;
            _mockRepo.Setup(repo => repo.GetAsync())
                     .ReturnsAsync(new List<Reservation>());

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchReservationException>(() => _reservationService.GetReservationsByCarId(carId));
            Assert.AreEqual($"No Reservation with the given id", ex.Message);
        }
       

        [Test]
        public void GetUserReservations_NoReservationsFound_ThrowsNoSuchReservationException()
        {
            // Arrange
            int userId = 1;
            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(new List<Reservation>());

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchReservationException>(() => _reservationService.GetUserReservations(userId));
            Assert.AreEqual($"No Reservation with the given id", ex.Message);
        }
        

        [Test]
        public void UpdateReservationPrice_ReservationNotFound_ThrowsNoSuchReservationException()
        {
            // Arrange
            int reservationId = 1;
            double newPrice = 150.0;
            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync((Reservation)null); // Returning null to simulate reservation not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchReservationException>(() => _reservationService.UpdateReservationPrice(reservationId, newPrice));
            Assert.AreEqual($"No Reservation with the given id", ex.Message);
        }

        [Test]
        public void UpdateReservationPrice_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            int reservationId = 1;
            double newPrice = 150.0;
            var reservation = new Reservation { ReservationId = reservationId, TotalPrice = 100.0 };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            _mockRepo.Setup(repo => repo.Update(reservation))
                     .ThrowsAsync(new Exception("Something went wrong during update")); // Simulating repository exception

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _reservationService.UpdateReservationPrice(reservationId, newPrice));
        }


        [Test]
        public void CancelReservation_ReservationNotFound_ThrowsNoSuchReservationException()
        {
            // Arrange
            int reservationId = 1;
            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync((Reservation)null); // Returning null to simulate reservation not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchReservationException>(() => _reservationService.CancelReservation(reservationId));
            Assert.AreEqual($"No Reservation with the given id", ex.Message);
        }

        [Test]
        public void CancelReservation_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            int reservationId = 1;
            var reservation = new Reservation { ReservationId = reservationId, Status = "Pending" };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            _mockRepo.Setup(repo => repo.Update(reservation))
                     .ThrowsAsync(new Exception("Something went wrong during update")); // Simulating repository exception

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _reservationService.CancelReservation(reservationId));
        }

        [Test]
        public async Task UpdateReservationStatus_ReservationFound_UpdatesStatus()
        {
            // Arrange
            int reservationId = 1;
            string newStatus = "Confirmed";
            var reservation = new Reservation { ReservationId = reservationId, Status = "Pending" };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            _mockRepo.Setup(repo => repo.Update(reservation))
                     .ReturnsAsync(reservation);

            // Act
            var result = await _reservationService.UpdateReservationStatus(reservationId, newStatus);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(newStatus, result.Status);
        }

        [Test]
        public void UpdateReservationStatus_ReservationNotFound_ThrowsNoSuchReservationException()
        {
            // Arrange
            int reservationId = 1;
            string newStatus = "Confirmed";

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync((Reservation)null); // Simulating reservation not found

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchReservationException>(() => _reservationService.UpdateReservationStatus(reservationId, newStatus));
            Assert.AreEqual($"No Reservation with the given id", ex.Message);
        }

        [Test]
        public void UpdateReservationStatus_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            int reservationId = 1;
            string newStatus = "Confirmed";
            var reservation = new Reservation { ReservationId = reservationId, Status = "Pending" };

            _mockRepo.Setup(repo => repo.GetAsyncById(reservationId))
                     .ReturnsAsync(reservation);

            _mockRepo.Setup(repo => repo.Update(reservation))
                     .ThrowsAsync(new Exception("Simulated repository exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _reservationService.UpdateReservationStatus(reservationId, newStatus));
        }

    }
}
