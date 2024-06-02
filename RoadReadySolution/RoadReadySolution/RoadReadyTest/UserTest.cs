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
    internal class UserTest
    {
        CarRentalDbContext _context;
        private UserService _userService;
        private Mock<IRepository<int, User>> _mockRepo;
        private Mock<ILogger<UserService>> _mockLogger;

        // <summary>
        /// Setup Method in which InMemory dummy database is created
        /// </summary>
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CarRentalDbContext>().UseInMemoryDatabase("dummyDatabase").Options;
            _context = new CarRentalDbContext(options);

            _mockRepo = new Mock<IRepository<int, User>>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockRepo.Object, _mockLogger.Object);
        }
       
        [Test]
        public async Task DeleteUserTest()
        {
            // Arrange
            const int userIdToDelete = 1;
            var userToDelete = new User
            {
                UserId = userIdToDelete,
                FirstName = "Kishor",
                LastName = "R",
                Email = "kishor.r@gmail.com",
                Username = "kishu_r",
                PhoneNumber = "1234567890",
                RegistrationDate = DateTime.Today
            };

            _mockRepo.Setup(repo => repo.Delete(userIdToDelete)).ReturnsAsync(userToDelete);

            // Act
            var result = await _userService.DeleteUser(userIdToDelete);

            // Assert
            Assert.That(result, Is.TypeOf<User>());
            Assert.That(result, Is.EqualTo(userToDelete));
        }


        [Test]
        public async Task GetAllUsers_ReturnsListOfUsers()
        {
            // Arrange
            var userList = new List<User>
        {
            new User { UserId = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Username = "johndoe", PhoneNumber = "1234567890", RegistrationDate = DateTime.Today },
            new User { UserId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Username = "janesmith", PhoneNumber = "9876543210", RegistrationDate = DateTime.Today }
           
        };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(userList);

            // Act
            var result = await _userService.GetAllUsers();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(userList));
            Assert.That(result.Count, Is.EqualTo(userList.Count));
        }

        [Test]
        public async Task GetUserById_ExistingUserId_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                // Set other properties as needed
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserById(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public void GetUserById_NonExistingUserId_ThrowsNoSuchUserException()
        {
            // Arrange
            var userId = 1;
            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.GetUserById(userId));
            Assert.That(ex.Message, Is.EqualTo("No user with the given id"));
        }

        [Test]
        public async Task GetUserPayments_ExistingUserId_ReturnsUserPayments()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Payments = new List<Payment>
            {
                new Payment { PaymentId = 1, PaymentAmount = 100 },
                new Payment { PaymentId = 2, PaymentAmount = 200 }
                
            }
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserPayments(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Payments.Count, result.Count);
        }

        [Test]
        public void GetUserPayments_NonExistingUserId_ThrowsNoSuchUserException()
        {
            // Arrange
            var userId = 1;
            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.GetUserPayments(userId));
            Assert.That(ex.Message, Is.EqualTo("No user with the given id"));
        }


        [Test]
        public async Task GetUserReservations_ExistingUserId_ReturnsUserReservations()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Reservations = new List<Reservation>
            {
                new Reservation { ReservationId = 1, PickUpDateTime = DateTime.Now.AddDays(1), Status ="Confirmed" },
                new Reservation { ReservationId = 2, PickUpDateTime = DateTime.Now.AddDays(2), Status = "Pending" }
                
            }
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserReservations(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Reservations.Count, result.Count);
            foreach (var reservation in user.Reservations)
            {
                Assert.Contains(reservation, result);
            }
        }

        [Test]
        public void GetUserReservations_NonExistingUserId_ThrowsNoSuchUserException()
        {
            // Arrange
            var userId = 1;
            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.GetUserReservations(userId));
            Assert.That(ex.Message, Is.EqualTo("No user with the given id"));
        }

        [Test]
        public async Task GetUserReviews_ExistingUserId_ReturnsUserReviews()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = userId,
                Reviews = new List<Review>
            {
                new Review { ReviewId = 1, Rating = 4, Comments = "Great experience" },
                new Review { ReviewId = 2, Rating = 5, Comments = "Excellent service" }
               
            }
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserReviews(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Reviews.Count, result.Count);
            foreach (var review in user.Reviews)
            {
                Assert.Contains(review, result);
            }
        }

        [Test]
        public void GetUserReviews_NonExistingUserId_ThrowsNoSuchUserException()
        {
            // Arrange
            var userId = 1;
            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.GetUserReviews(userId));
            Assert.That(ex.Message, Is.EqualTo("No user with the given id"));
        }

       

        [Test]
        public async Task UpdateEmail_ExistingUserId_UpdatesUserEmail()
        {
            // Arrange
            var userId = 1;
            var userEmail = "kishu@gmail.com";
            var user = new User
            {
                UserId = userId,
                Email = "swat@gmail.com"
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.UpdateEmail(userId, userEmail);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userEmail, result.Email);
            _mockRepo.Verify(repo => repo.Update(user), Times.Once);
        }

        [Test]
        public void UpdateEmail_NonExistingUserId_ThrowsNoSuchUserException()
        {
            // Arrange
            var userId = 1;
            var userEmail = "sk@example.com";
            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.UpdateEmail(userId, userEmail));
            Assert.That(ex.Message, Is.EqualTo("No user with the given id"));
        }

        [Test]
        public async Task UpdatePassword_ExistingUserId_UpdatesUserPassword()
        {
            // Arrange
            var userId = 1;
            var password = new byte[] { 1, 2, 3, 4 }; 
            var user = new User
            {
                UserId = userId,
                Password = new byte[] { 5, 6, 7, 8 } 
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.UpdatePassword(userId, password);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(password, result.Password);
            _mockRepo.Verify(repo => repo.Update(user), Times.Once);
        }

        [Test]
        public void UpdatePassword_NonExistingUserId_ThrowsNoSuchUserException()
        {
            // Arrange
            var userId = 1;
            var password = new byte[] { 1, 2, 3, 4 }; // example password bytes
            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.UpdatePassword(userId, password));
            Assert.That(ex.Message, Is.EqualTo("No user with the given id"));
        }

      
        [Test]
        public async Task UpdatePhoneNumber_ExistingUserId_UpdatesUserPhoneNumber()
        {
            // Arrange
            var userId = 1;
            var phoneNumber = "1234567890";
            var user = new User
            {
                UserId = userId,
                PhoneNumber = "0987654321" // example old phone number
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.UpdatePhoneNumber(userId, phoneNumber);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(phoneNumber, result.PhoneNumber);
            _mockRepo.Verify(repo => repo.Update(user), Times.Once);
        }

        [Test]
        public void UpdatePhoneNumber_NonExistingUserId_ThrowsNoSuchUserException()
        {
            // Arrange
            var userId = 1;
            var phoneNumber = "1234567890";
            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.UpdatePhoneNumber(userId, phoneNumber));
            Assert.That(ex.Message, Is.EqualTo("No user with the given id"));
        }


        [Test]
        public async Task UpdateUserName_ExistingUserId_UpdatesUserName()
        {
            // Arrange
            var userId = 1;
            var userName = "skleo";
            var user = new User
            {
                UserId = userId,
                Username = "afra" // example old username
            };

            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.UpdateUserName(userId, userName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userName, result.Username);
            _mockRepo.Verify(repo => repo.Update(user), Times.Once);
        }

        [Test]
        public void UpdateUserName_NonExistingUserId_ThrowsNoSuchUserException()
        {
            // Arrange
            var userId = 1;
            var userName = "newusername";
            _mockRepo.Setup(repo => repo.GetAsyncById(userId)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.UpdateUserName(userId, userName));
            Assert.That(ex.Message, Is.EqualTo("No user with the given id"));
        }
        [Test]
        public async Task GetUserByName_ExistingUserName_ReturnsUser()
        {
            // Arrange
            var userName = "existingUser";
            var user = new User
            {
                UserId = 1,
                Username = userName
                // Set other properties as needed
            };

            _mockRepo.Setup(repo => repo.GetAsyncByName(userName)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByName(userName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userName, result.Username);
        }

        [Test]
        public void GetUserByName_NonExistingUserName_ThrowsNoSuchUserException()
        {
            // Arrange
            var userName = "nonExistingUser";
            _mockRepo.Setup(repo => repo.GetAsyncByName(userName)).ReturnsAsync((User)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.GetUserByName(userName));
            Assert.That(ex.Message, Is.EqualTo("No user with the given id"));
        }

        [Test]
        public async Task GetUserByName_NullUserName_ThrowsArgumentException()
        {
            // Arrange
            string nullUserName = null;

            // Act & Assert
            Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.GetUserByName(nullUserName));
        }

        [Test]
        public async Task GetUserByName_EmptyUserName_ThrowsArgumentException()
        {
            // Arrange
            string emptyUserName = "";

            // Act & Assert
            Assert.ThrowsAsync<NoSuchUserException>(async () => await _userService.GetUserByName(emptyUserName));
        }



    }
}
