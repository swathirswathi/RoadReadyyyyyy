using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using RoadReady.Contexts;
using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using RoadReady.Services;
using System.Data;
using System.Text;

namespace RoadReadyTest
{
    public class AdminTest
    {
        CarRentalDbContext _context;
        private AdminService _adminService;
        private Mock<IRepository<int, Admin>> _mockRepo;
        private Mock<ILogger<AdminService>> _mockLogger;


        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CarRentalDbContext>().UseInMemoryDatabase("dummyDatabase").Options;
            _context = new CarRentalDbContext(options);

            _mockRepo = new Mock<IRepository<int, Admin>>();
            _mockLogger = new Mock<ILogger<AdminService>>();
            _adminService = new AdminService(_mockRepo.Object, _mockLogger.Object);
        }

       
        [Test]
        public async Task DeleteAdminTest()
        {
            // Arrange
            const int adminIdToDelete = 1;
            var adminToDelete = new Admin
            {
                AdminId = adminIdToDelete,
                FirstName = "Swathi",
                LastName = "R",
                Email = "swathir@gmail.com",
                Username = "swathi_r",
                Password = new byte[] { 0x01, 0x02, 0x03 }, // Sample password bytes
                PhoneNumber = "1234567890"
            };

            _mockRepo.Setup(repo => repo.Delete(adminIdToDelete)).ReturnsAsync(adminToDelete);

            // Act
            var result = await _adminService.DeleteAdmin(adminIdToDelete);

            // Assert
            Assert.That(result, Is.TypeOf<Admin>());
            Assert.That(result, Is.EqualTo(adminToDelete));
        }

        [Test]
        public async Task GetAllAdminsTest()
        {
            // Arrange
            var adminList = new List<Admin>
            {
                new Admin { AdminId = 1, FirstName = "John", LastName = "Doe" },
                new Admin { AdminId = 2, FirstName = "Jane", LastName = "Smith" },
               
            };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(adminList);

            // Act
            var result = await _adminService.GetAllAdmins();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<Admin>>(result);
            Assert.AreEqual(adminList, result);
        }

        [Test]
        public async Task GetAdminByIdTest()
        {
            // Arrange
            const int adminIdToRetrieve = 1;
            var adminToRetrieve = new Admin { AdminId = adminIdToRetrieve, FirstName = "John", LastName = "Doe" };

            _mockRepo.Setup(repo => repo.GetAsyncById(adminIdToRetrieve)).ReturnsAsync(adminToRetrieve);

            // Act
            var result = await _adminService.GetAdminById(adminIdToRetrieve);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Admin>(result);
            Assert.AreEqual(adminToRetrieve, result);
        }
        [Test]
        public async Task UpdateAdminEmail_ValidInput_ReturnsUpdatedAdmin()
        {
            // Arrange
            int adminId = 1;
            string newEmail = "swathir@gmail.com";
            var existingAdmin = new Admin { AdminId = adminId, FirstName = "kishu", LastName = "R", Email = "kishu@gmail.com" };

            // Setup the mock repository
            _mockRepo.Setup(repo => repo.GetAsyncById(adminId)).ReturnsAsync(existingAdmin);

            // Act
            var result = await _adminService.UpdateAdminEmail(adminId, newEmail);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AdminId, Is.EqualTo(adminId));
            Assert.That(result.Email, Is.EqualTo(newEmail));
        }

        [Test]
        public async Task UpdateAdminEmail_AdminNotFound_ThrowsNoSuchAdminException()
        {
            // Arrange
            int adminId = 1;
            string newEmail = "swathir@gmail.com";

            // Setup the mock repository to return null, simulating admin not found
            _mockRepo.Setup(repo => repo.GetAsyncById(adminId)).ReturnsAsync((Admin)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchAdminException>(() => _adminService.UpdateAdminEmail(adminId, newEmail));
        }

        [Test]
        public async Task UpdateAdminEmail_NullOrEmptyEmail_ThrowsArgumentException()
        {
            // Arrange
            int adminId = 1;
            string invalidEmail = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => _adminService.UpdateAdminEmail(adminId, invalidEmail));
        }
        [Test]
        public async Task UpdateAdminPassword_ValidInput_Success()
        {
            // Arrange
            int adminId = 1;
            byte[] newPassword = Encoding.UTF8.GetBytes("Swathi234"); // Example new password
            Admin existingAdmin = new Admin { AdminId = adminId, Password = Encoding.UTF8.GetBytes("oldPassword") }; // Existing admin with an old password
            _mockRepo.Setup(repo => repo.GetAsyncById(adminId)).ReturnsAsync(existingAdmin); // Setup mock repository to return the existing admin

            // Act
            var result = await _adminService.UpdateAdminPassword(adminId, newPassword);

            // Assert
            Assert.IsNotNull(result); // Ensure result is not null
            Assert.AreEqual(adminId, result.AdminId); // Ensure AdminId is unchanged
            Assert.AreEqual(newPassword, result.Password); // Ensure password is updated correctly
        }
        [Test]
        public async Task UpdateAdminPhoneNumber_Valid()
        {
            // Arrange
            int adminId = 1;
            string newPhoneNumber = "1234567890"; // Example new phone number
            var adminToUpdate = new Admin { AdminId = adminId, PhoneNumber = "98765432345" }; // Create a sample admin entity
            _mockRepo.Setup(repo => repo.GetAsyncById(adminId)).ReturnsAsync(adminToUpdate); // Mock repository method to return the sample admin entity

            // Act
            var updatedAdmin = await _adminService.UpdateAdminPhoneNumber(adminId, newPhoneNumber);

            // Assert
            Assert.IsNotNull(updatedAdmin);
            Assert.AreEqual(adminId, updatedAdmin.AdminId);
            Assert.AreEqual(newPhoneNumber, updatedAdmin.PhoneNumber);
        }
        [Test]
        public async Task UpdateAdminPhoneNumber_NullPhoneNumber_ThrowsArgumentException()
        {
            // Arrange
            int adminId = 1;
            string nullPhoneNumber = null; // Null phone number
            var adminToUpdate = new Admin { AdminId = adminId, PhoneNumber = "9876543455" }; // Sample admin entity
            _mockRepo.Setup(repo => repo.GetAsyncById(adminId)).ReturnsAsync(adminToUpdate); // Mock repository method

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _adminService.UpdateAdminPhoneNumber(adminId, nullPhoneNumber);
            });
        }

        [Test]
        public async Task UpdateAdminPhoneNumber_EmptyPhoneNumber_ThrowsArgumentException()
        {
            // Arrange
            int adminId = 1;
            string emptyPhoneNumber = ""; // Empty phone number
            var adminToUpdate = new Admin { AdminId = adminId, PhoneNumber = "987654567" }; // Sample admin entity
            _mockRepo.Setup(repo => repo.GetAsyncById(adminId)).ReturnsAsync(adminToUpdate); // Mock repository method

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                // Act
                await _adminService.UpdateAdminPhoneNumber(adminId, emptyPhoneNumber);
            });
        }

        [Test]
        public async Task UpdateAdminUserName_AdminNotFound_ThrowsNoSuchAdminException()
        {
            // Arrange
            int nonExistingAdminId = 999;
            string validUserName = "swat2723";

            // Act & Assert
            Assert.ThrowsAsync<NoSuchAdminException>(async () =>
            {
                await _adminService.UpdateAdminUserName(nonExistingAdminId, validUserName);
            });
        }

        [Test]
        public async Task UpdateAdminUserName_ValidInput_Success()
        {
            // Arrange
            int adminId = 1;
            string newUserName = "newUserName";
            var existingAdmin = new Admin { AdminId = adminId, Username = "oldUsername" };
            _mockRepo.Setup(repo => repo.GetAsyncById(adminId)).ReturnsAsync(existingAdmin);

            // Act
            var result = await _adminService.UpdateAdminUserName(adminId, newUserName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(adminId, result.AdminId);
            Assert.AreEqual(newUserName, result.Username);
        }

        [Test]
        public async Task UpdateAdmin_InvalidAdminId_ThrowsNoSuchAdminException()
        {
            // Arrange
            int nonExistingAdminId = 999;
            string newUserName = "newUserName";

            // Act & Assert
            Assert.ThrowsAsync<NoSuchAdminException>(() => _adminService.UpdateAdminUserName(nonExistingAdminId, newUserName));
        }

        [Test]
        public async Task GetAdminById_ReturnsAdmin_WhenAdminFound()
        {
            // Arrange
            int adminId = 1;
            var adminToRetrieve = new Admin { AdminId = adminId, FirstName = "John", LastName = "Doe" };
            _mockRepo.Setup(repo => repo.GetAsyncById(adminId)).ReturnsAsync(adminToRetrieve);

            // Act
            var result = await _adminService.GetAdminById(adminId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(adminId, result.AdminId);
            Assert.AreEqual("John", result.FirstName);
            Assert.AreEqual("Doe", result.LastName);
        }

        [Test]
        public void GetAdminById_ThrowsNoSuchAdminException_WhenAdminNotFound()
        {
            // Arrange
            int adminId = 2;
            _mockRepo.Setup(repo => repo.GetAsyncById(adminId)).ReturnsAsync((Admin)null);

            // Act & Assert
            Assert.ThrowsAsync<NoSuchAdminException>(() => _adminService.GetAdminById(adminId));
        }

        [Test]
        public void GetAllAdmins_ThrowsException_WhenRepositoryThrowsException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAsync()).ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(() => _adminService.GetAllAdmins());
            Assert.AreEqual("Repository error", exception.Message);
        }

        [Test]
        public async Task GetAdminByName_ReturnsAdmin_WhenAdminExists()
        {
            // Arrange
            string existingUserName = "existingUser";
            var existingAdmin = new Admin { Username = existingUserName };
            _mockRepo.Setup(repo => repo.GetAsyncByName(existingUserName)).ReturnsAsync(existingAdmin);

            // Act
            var result = await _adminService.GetAdminByName(existingUserName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(existingAdmin, result);
        }

        [Test]
        public void GetAdminByName_ThrowsNoSuchAdminException_WhenAdminDoesNotExist()
        {
            // Arrange
            string nonExistingUserName = "nonExistingUser";
            _mockRepo.Setup(repo => repo.GetAsyncByName(nonExistingUserName)).ReturnsAsync((Admin)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<NoSuchAdminException>(() => _adminService.GetAdminByName(nonExistingUserName));
            Assert.AreEqual($"No admin with the given id", exception.Message);
        }

        [Test]
        public void GetAdminByName_ThrowsArgumentNullException_WhenUserNameIsNull()
        {
            // Arrange
            string nullUserName = null;

            // Act & Assert
            Assert.ThrowsAsync<NoSuchAdminException>(() => _adminService.GetAdminByName(nullUserName));
        }


    }

}