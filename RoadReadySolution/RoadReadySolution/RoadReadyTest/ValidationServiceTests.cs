using System;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using RoadReady.Exceptions;
using RoadReady.Interface;
using RoadReady.Models;
using RoadReady.Models.DTO;
using RoadReady.Services;
namespace RoadReadyTest
{

    public class ValidationServiceTests
    {
        private ValidationService _validationService;
        private Mock<IRepository<int, Admin>> _adminRepositoryMock;
        private Mock<IRepository<int, User>> _userRepositoryMock;
        private Mock<IRepository<string, Validation>> _validationRepositoryMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<ILogger<ValidationService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _adminRepositoryMock = new Mock<IRepository<int, Admin>>();
            _userRepositoryMock = new Mock<IRepository<int, User>>();
            _validationRepositoryMock = new Mock<IRepository<string, Validation>>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<ValidationService>>();

            _validationService = new ValidationService(
                _adminRepositoryMock.Object,
                _userRepositoryMock.Object,
                _validationRepositoryMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object
            );
        }



        [Test]
        public void Login_InvalidValidation_ThrowsInvalidValidationException()
        {
            // Arrange
            var validationDto = new LoginValidationDto
            {
                Username = "sudharani",
                Password = "sudharani"
                // Set other properties as needed
            };

            _validationRepositoryMock.Setup(repo => repo.GetAsyncById(validationDto.Username))
                .ReturnsAsync((Validation)null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidValidationException>(() => _validationService.Login(validationDto));
        }

        [Test]
        public async Task RegisterUser_ValidInput_ReturnsLoginValidationDto()
        {
            // Arrange
            var registerUserDto = new RegisterUserDto
            {
                Username = "validUsername",
                Password = "validPassword"

            };

            var validationEntity = new Validation
            {
                Username = "validUsername",
                Password = new byte[] { 0x01, 0x02, 0x03 },
                Key = new byte[] { 0x01, 0x02, 0x03 },
                Role = "user"

            };

            _validationRepositoryMock.Setup(repo => repo.Add(It.IsAny<Validation>()))
                .ReturnsAsync(validationEntity);

            _userRepositoryMock.Setup(repo => repo.Add(It.IsAny<User>()))
                .ReturnsAsync(new User()); // Mock the user repository as needed

            // Act
            var result = await _validationService.RegisterUser(registerUserDto);

            // Assert
            Assert.AreEqual(registerUserDto.Username, result.Username);
            Assert.AreEqual(validationEntity.Role, result.Role);
        }

        [Test]
        public async Task RegisterAdmin_ValidInput_ReturnsLoginValidationDto()
        {
            // Arrange
            var registerAdminDto = new RegisterAdminDto
            {
                Username = "validAdminUsername",
                Password = "validAdminPassword"

            };

            var validationEntity = new Validation
            {
                Username = "validAdminUsername",
                Password = new byte[] { 0x01, 0x02, 0x03 },
                Key = new byte[] { 0x01, 0x02, 0x03 },
                Role = "admin"

            };

            _validationRepositoryMock.Setup(repo => repo.Add(It.IsAny<Validation>()))
                .ReturnsAsync(validationEntity);

            _adminRepositoryMock.Setup(repo => repo.Add(It.IsAny<Admin>()))
                .ReturnsAsync(new Admin()); // Mock the admin repository as needed

            // Act
            var result = await _validationService.RegisterAdmin(registerAdminDto);

            // Assert
            Assert.AreEqual(registerAdminDto.Username, result.Username);
            Assert.AreEqual(validationEntity.Role, result.Role);
        }

        [Test]
        public void ComparePasswords_PasswordsMatch_ReturnsTrue()
        {
            // Arrange
            byte[] password = { 0x01, 0x02, 0x03 };
            byte[] validationPassword = { 0x01, 0x02, 0x03 };

            // Act
            bool result = ComparePasswords(password, validationPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ComparePasswords_PasswordsDoNotMatch_ReturnsFalse()
        {
            // Arrange
            byte[] password = { 0x01, 0x02, 0x03 };
            byte[] validationPassword = { 0x04, 0x05, 0x06 };

            // Act
            bool result = ComparePasswords(password, validationPassword);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void GetPasswordEncrypted_ValidInput_ReturnsNonEmptyByteArray()
        {
            // Arrange
            string password = "testPassword";
            byte[] key = Encoding.UTF8.GetBytes("testKey");

            // Act
            byte[] result = GetPasswordEncrypted(password, key);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        private bool ComparePasswords(byte[] password, byte[] validationPassword)
        {
            for (int i = 0; i < password.Length; i++)
            {
                if (password[i] != validationPassword[i])
                    return false;
            }
            return true;
        }

        private byte[] GetPasswordEncrypted(string password, byte[] key)
        {
            using (HMACSHA512 hmac = new HMACSHA512(key))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        [Test]
        public void GetPasswordEncrypted_ValidInput_ReturnsEncryptedPassword()
        {
            // Arrange
            string password = "testPassword";
            byte[] key = Encoding.UTF8.GetBytes("testKey");
            ValidationService validationService = new ValidationService(
                _adminRepositoryMock.Object,
                _userRepositoryMock.Object,
                _validationRepositoryMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object
            );

            // Use reflection to access the private method
            MethodInfo methodInfo = typeof(ValidationService).GetMethod("GetPasswordEncrypted", BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                Assert.Fail("GetPasswordEncrypted method not found.");
            }

            // Act
            byte[] encryptedPassword = (byte[])methodInfo.Invoke(validationService, new object[] { password, key });

            // Assert
            Assert.IsNotNull(encryptedPassword);
            Assert.AreEqual(64, encryptedPassword.Length); // SHA-512 hash length is 64 bytes
            // Add more specific assertions if needed
        }


        

        

        [Test]
        public void Login_InvalidCredentials_ThrowsInvalidValidationException()
        {
            // Arrange
            var username = "testUser";
            var password = "testPassword";

            _validationRepositoryMock.Setup(repo => repo.GetAsyncById(username))
                .ReturnsAsync((Validation)null);

            var loginValidationDto = new LoginValidationDto
            {
                Username = username,
                Password = password
            };

            // Act & Assert
            Assert.ThrowsAsync<InvalidValidationException>(() => _validationService.Login(loginValidationDto));
        }

       

        [Test]
        public void Login_InvalidUsername_ThrowsInvalidValidationException()
        {
            // Arrange
            var invalidUsername = "nonexistentUser";
            var loginValidationDto = new LoginValidationDto
            {
                Username = invalidUsername,
                Password = "testPassword"
            };

            _validationRepositoryMock.Setup(repo => repo.GetAsyncById(invalidUsername))
                .ReturnsAsync((Validation)null);

            // Act & Assert
            Assert.ThrowsAsync<InvalidValidationException>(() => _validationService.Login(loginValidationDto));
        }

        [Test]
        public void Login_InvalidPassword_ThrowsInvalidValidationException()
        {
            // Arrange
            var username = "testUser";
            var incorrectPassword = "wrongPassword";

            var validation = new Validation
            {
                Username = username,
                Password = new byte[] { 0x01, 0x02, 0x03 }, // Mock hashed password value
                Key = new byte[] { 0x01, 0x02, 0x03 }, // Mock key value
                Role = "user"
            };

            var loginValidationDto = new LoginValidationDto
            {
                Username = username,
                Password = incorrectPassword
            };

            _validationRepositoryMock.Setup(repo => repo.GetAsyncById(username))
                .ReturnsAsync(validation);

            // Act & Assert
            Assert.ThrowsAsync<InvalidValidationException>(() => _validationService.Login(loginValidationDto));
        }

    }
}
