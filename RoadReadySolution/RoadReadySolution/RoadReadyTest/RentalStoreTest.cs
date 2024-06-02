using Microsoft.Extensions.Logging;
using Moq;
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
    internal class RentalStoreTest
    {
        private Mock<IRepository<int, RentalStore>> _mockRentalStoreRepository;
        private Mock<ILogger<RentalStoreService>> _mockLogger;
        private RentalStoreService _rentalStoreService;

        [SetUp]
        public void Setup()
        {
            _mockRentalStoreRepository = new Mock<IRepository<int, RentalStore>>();
            _mockLogger = new Mock<ILogger<RentalStoreService>>();
            _rentalStoreService = new RentalStoreService(_mockRentalStoreRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task AddRentalStore_ValidInput_ReturnsRentalStore()
        {
            // Arrange
            var rentalStoreToAdd = new RentalStore { StoreId = 1, PickUpStoreLocation = "Location1", DropOffStoreLocation = "Location2" };
            _mockRentalStoreRepository.Setup(repo => repo.Add(rentalStoreToAdd)).ReturnsAsync(rentalStoreToAdd);

            // Act
            var addedRentalStore = await _rentalStoreService.AddRentalStore(rentalStoreToAdd);

            // Assert
            Assert.IsNotNull(addedRentalStore);
            Assert.AreEqual(rentalStoreToAdd, addedRentalStore);
        }

        [Test]
        public async Task RemoveRentalStore_ValidId_ReturnsRemovedRentalStore()
        {
            // Arrange
            var rentalStoreToRemove = new RentalStore { StoreId = 1, PickUpStoreLocation = "Location1", DropOffStoreLocation = "Location2" };
            _mockRentalStoreRepository.Setup(repo => repo.Delete(rentalStoreToRemove.StoreId)).ReturnsAsync(rentalStoreToRemove);

            // Act
            var removedRentalStore = await _rentalStoreService.RemoveRentalStore(rentalStoreToRemove.StoreId);

            // Assert
            Assert.IsNotNull(removedRentalStore);
            Assert.AreEqual(rentalStoreToRemove, removedRentalStore);
        }

        [Test]
        public async Task GetAllRentalStores_ReturnsListOfRentalStores()
        {
            // Arrange
            var rentalStoresList = new List<RentalStore>
        {
            new RentalStore { StoreId = 1, PickUpStoreLocation = "Location1", DropOffStoreLocation = "Location2" },
            new RentalStore { StoreId = 2, PickUpStoreLocation = "Location3", DropOffStoreLocation = "Location4" }
        };
            _mockRentalStoreRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(rentalStoresList);

            // Act
            var allRentalStores = await _rentalStoreService.GetAllRentalStores();

            // Assert
            Assert.IsNotNull(allRentalStores);
            Assert.AreEqual(rentalStoresList, allRentalStores);
        }


        [Test]
        public async Task GetRentalStoreById_ExistingId_ReturnsRentalStore()
        {
            // Arrange
            var existingStoreId = 1;
            var existingRentalStore = new RentalStore { StoreId = existingStoreId, PickUpStoreLocation = "Location1", DropOffStoreLocation = "Location2" };
            _mockRentalStoreRepository.Setup(repo => repo.GetAsyncById(existingStoreId)).ReturnsAsync(existingRentalStore);

            // Act
            var resultRentalStore = await _rentalStoreService.GetRentalStoreById(existingStoreId);

            // Assert
            Assert.IsNotNull(resultRentalStore);
            Assert.AreEqual(existingRentalStore, resultRentalStore);
        }

        [Test]
        public async Task GetCarsInStore_ExistingStoreId_ReturnsListOfCarStores()
        {
            // Arrange
            var existingStoreId = 1;
            var rentalStoreWithCars = new RentalStore
            {
                StoreId = existingStoreId,
                CarStore = new List<CarStore>
    {
        new CarStore { CarId = 1, StoreId= 1 },
        new CarStore { CarId = 2, StoreId =2 }
    }
            };
            _mockRentalStoreRepository.Setup(repo => repo.GetAsyncById(existingStoreId)).ReturnsAsync(rentalStoreWithCars);

            // Act
            var resultCarStores = await _rentalStoreService.GetCarsInStore(existingStoreId);

            // Assert
            Assert.IsNotNull(resultCarStores);
            Assert.AreEqual(rentalStoreWithCars.CarStore.ToList(), resultCarStores);
        }
       
        [Test]
        public async Task GetCarsInStore_NoCarsInStore_ThrowsNoSuchCarException()
        {
            // Arrange
            var existingStoreId = 1;
            var rentalStoreWithoutCars = new RentalStore { StoreId = existingStoreId, CarStore = null };
            _mockRentalStoreRepository.Setup(repo => repo.GetAsyncById(existingStoreId)).ReturnsAsync(rentalStoreWithoutCars);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCarException>(() => _rentalStoreService.GetCarsInStore(existingStoreId));
        }

        [Test]
        public async Task GetCarsInStore_StoreNotFound_ThrowsNoSuchCarException()
        {
            // Arrange
            var nonExistingStoreId = 999;
            _mockRentalStoreRepository.Setup(repo => repo.GetAsyncById(nonExistingStoreId)).ReturnsAsync((RentalStore)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchCarException>(() => _rentalStoreService.GetCarsInStore(nonExistingStoreId));
        }


        [Test]
        public async Task GetRentalStoreById_NonExistingId_ThrowsNoSuchRentalStoreException()
        {
            // Arrange
            var nonExistingStoreId = 999;
            _mockRentalStoreRepository.Setup(repo => repo.GetAsyncById(nonExistingStoreId)).ReturnsAsync((RentalStore)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchRentalStoreException>(() => _rentalStoreService.GetRentalStoreById(nonExistingStoreId));
        }

        [Test]
        public async Task UpdateRentalStoreDetails_ExistingStoreId_ReturnsUpdatedRentalStore()
        {
            // Arrange
            var existingStoreId = 1;
            var rentalStoreToUpdate = new RentalStore { StoreId = existingStoreId, PickUpStoreLocation = "Location1", DropOffStoreLocation = "Location2" };
            var updatedPickUpLocation = "NewLocation";

            _mockRentalStoreRepository.Setup(repo => repo.GetAsyncById(existingStoreId)).ReturnsAsync(rentalStoreToUpdate);

            // Act
            var updatedRentalStore = await _rentalStoreService.UpdateRentalStoreDetails(existingStoreId, updatedPickUpLocation, rentalStoreToUpdate.DropOffStoreLocation);

            // Assert
            Assert.IsNotNull(updatedRentalStore);
            Assert.AreEqual(updatedPickUpLocation, updatedRentalStore.PickUpStoreLocation);
        }

        [Test]
        public async Task UpdateRentalStoreDetails_NonExistingStoreId_ThrowsNoSuchRentalStoreException()
        {
            // Arrange
            var nonExistingStoreId = 999;
            var updatedPickUpLocation = "NewLocation";

            _mockRentalStoreRepository.Setup(repo => repo.GetAsyncById(nonExistingStoreId)).ReturnsAsync((RentalStore)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchRentalStoreException>(() => _rentalStoreService.UpdateRentalStoreDetails(nonExistingStoreId, updatedPickUpLocation, "Location2"));
        }

        [Test]
        public async Task AddRentalStore_RentalStoreAlreadyExists_ThrowsRentalStoreAlreadyExistsException()
        {
            // Arrange
            var rentalStore = new RentalStore { StoreId = 1, PickUpStoreLocation = "Location1", DropOffStoreLocation = "Location2" };
            _mockRentalStoreRepository.Setup(repo => repo.Add(rentalStore)).ThrowsAsync(new RentalStoreAlreadyExistsException());

            // Act & Assert
            var ex = Assert.ThrowsAsync<RentalStoreAlreadyExistsException>(() => _rentalStoreService.AddRentalStore(rentalStore));
            Assert.AreEqual($"RentalStore already exists.", ex.Message);
        }

        [Test]
        public async Task AddRentalStore_GenericExceptionThrown_LogsErrorAndRethrows()
        {
            // Arrange
            var rentalStore = new RentalStore { StoreId = 1, PickUpStoreLocation = "Location1", DropOffStoreLocation = "Location2" };
            _mockRentalStoreRepository.Setup(repo => repo.Add(rentalStore)).ThrowsAsync(new Exception("Something went wrong"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _rentalStoreService.AddRentalStore(rentalStore));
            Assert.AreEqual($"Something went wrong", ex.Message);
        }

    }
}

