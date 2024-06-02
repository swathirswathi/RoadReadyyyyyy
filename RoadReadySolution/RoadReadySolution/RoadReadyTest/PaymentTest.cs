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
    internal class PaymentTest
    {
        private CarRentalDbContext context;
        private Mock<IRepository<int, Payment>> _mockRepo;
        private Mock<ILogger<PaymentService>> _mockLogger;
        private PaymentService _paymentService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<CarRentalDbContext>()
                .UseInMemoryDatabase("dummyDatabase")
                .Options;

            context = new CarRentalDbContext(options);

            _mockRepo = new Mock<IRepository<int, Payment>>();
            _mockLogger = new Mock<ILogger<PaymentService>>();
            _paymentService = new PaymentService(_mockRepo.Object, _mockLogger.Object);
        }

        [Test]
        public async Task MakePayment_ValidPayment_AddsPayment()
        {
            // Arrange
            var payment = new Payment { PaymentId = 1, PaymentAmount = 100.00};

            _mockRepo.Setup(repo => repo.Add(payment)).ReturnsAsync(payment);

            // Act
            var result = await _paymentService.MakePayment(payment);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(payment, result);
            _mockRepo.Verify(repo => repo.Add(payment), Times.Once);
        }

        [Test]
        public async Task CancelPayment_ExistingPayment_CancelsPayment()
        {
            // Arrange
            var paymentId = 1;
            var payment = new Payment { PaymentId = paymentId, PaymentStatus = "Pending" };

            _mockRepo.Setup(repo => repo.GetAsyncById(paymentId)).ReturnsAsync(payment);
            _mockRepo.Setup(repo => repo.Update(payment)).ReturnsAsync(payment);

            // Act
            var result = await _paymentService.CancelPayment(paymentId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Cancelled", result.PaymentStatus);
            _mockRepo.Verify(repo => repo.Update(payment), Times.Once);
        }

        [Test]
        public void CancelPayment_NonExistingPayment_ThrowsNoSuchPaymentException()
        {
            // Arrange
            var paymentId = 1;
            _mockRepo.Setup(repo => repo.GetAsyncById(paymentId)).ReturnsAsync((Payment)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchPaymentException>(async () => await _paymentService.CancelPayment(paymentId));
            Assert.That(ex.Message, Is.EqualTo($"No payment with the given id"));
        }

        [Test]
        public async Task GetPaymentDetails_ExistingPayment_ReturnsPayment()
        {
            // Arrange
            var paymentId = 1;
            var payment = new Payment { PaymentId = paymentId };

            _mockRepo.Setup(repo => repo.GetAsyncById(paymentId)).ReturnsAsync(payment);

            // Act
            var result = await _paymentService.GetPaymentDetails(paymentId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(payment, result);
            _mockRepo.Verify(repo => repo.GetAsyncById(paymentId), Times.Once);
        }

        [Test]
        public async Task GetPaymentHistoryList_ReturnsListOfPayments()
        {
            // Arrange
            var payments = new List<Payment>
        {
            new Payment { PaymentId = 1, PaymentAmount = 100.00},
            new Payment { PaymentId = 2, PaymentAmount = 50.00 },
            // Add more payments as needed
        };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(payments);

            // Act
            var result = await _paymentService.GetPaymentHistoryList();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(payments.Count, result.Count);
            Assert.AreEqual(payments, result);
            _mockRepo.Verify(repo => repo.GetAsync(), Times.Once);
        }

        [Test]
        public async Task UpdatePaymentStatus_ExistingPayment_UpdatesPaymentStatus()
        {
            // Arrange
            var paymentId = 1;
            var paymentStatus = "Paid";
            var payment = new Payment { PaymentId = paymentId, PaymentStatus = "Pending" };

            _mockRepo.Setup(repo => repo.GetAsyncById(paymentId)).ReturnsAsync(payment);
            _mockRepo.Setup(repo => repo.Update(payment)).ReturnsAsync(payment);

            // Act
            var result = await _paymentService.UpdatePaymentStatus(paymentId, paymentStatus);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(paymentStatus, result.PaymentStatus);
            _mockRepo.Verify(repo => repo.Update(payment), Times.Once);
        }


        [Test]
        public async Task GetPendingPayments_ReturnsListOfPendingPayments()
        {
            // Arrange
            var pendingPayments = new List<Payment>
        {
            new Payment { PaymentId = 1, PaymentStatus = "Pending" },
            new Payment { PaymentId = 2, PaymentStatus = "Pending" },
           
        };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(pendingPayments);

            // Act
            var result = await _paymentService.GetPendingPayments();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(pendingPayments.Count, result.Count);
            Assert.AreEqual(pendingPayments, result);
            _mockRepo.Verify(repo => repo.GetAsync(), Times.Once);
        }

        [Test]
        public void GetPendingPayments_EmptyList_ThrowsNoSuchPaymentException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(new List<Payment>());

            // Act & Assert
            var ex = Assert.ThrowsAsync<NoSuchPaymentException>(async () => await _paymentService.GetPendingPayments());
            Assert.That(ex.Message, Is.EqualTo("No payment with the given id"));
        }



        [Test]
        public async Task GetUserPayments_ExistingPayments_ReturnsUserPayments()
        {
            // Arrange
            var userId = 1;
            var userPayments = new List<Payment>
    {
        new Payment { UserId = userId, PaymentId = 1 },
        new Payment { UserId = userId, PaymentId = 2 }
    };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(userPayments);

            // Act
            var result = await _paymentService.GetUserPayments(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userPayments.Count, result.Count);
            Assert.AreEqual(userPayments, result);
        }

        [Test]
        public async Task MakePayment_ValidPayment_ReturnsPayment()
        {
            // Arrange
            var payment = new Payment { PaymentId = 1, PaymentAmount = 100.0 };

            _mockRepo.Setup(repo => repo.Add(payment)).ReturnsAsync(payment);

            // Act
            var result = await _paymentService.MakePayment(payment);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(payment, result);
        }

        [Test]
        public void MakePayment_PaymentAlreadyExists_ThrowsPaymentAlreadyExistsException()
        {
            // Arrange
            var payment = new Payment { PaymentId = 1, PaymentAmount = 100.0 };

            _mockRepo.Setup(repo => repo.Add(payment)).ThrowsAsync(new PaymentAlreadyExistsException());

            // Act & Assert
            Assert.ThrowsAsync<PaymentAlreadyExistsException>(async () => await _paymentService.MakePayment(payment));
        }

        [Test]
        public void MakePayment_ExceptionThrown_ThrowsException()
        {
            // Arrange
            var payment = new Payment { PaymentId = 1, PaymentAmount = 100.0 };

            _mockRepo.Setup(repo => repo.Add(payment)).ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _paymentService.MakePayment(payment));
        }

        [Test]
        public void GetPaymentDetails_ExceptionThrown_ThrowsException()
        {
            // Arrange
            var paymentId = 1;

            _mockRepo.Setup(repo => repo.GetAsyncById(paymentId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _paymentService.GetPaymentDetails(paymentId));
        }

        [Test]
        public async Task GetPaymentHistoryList_NotEmptyList_ReturnsPaymentList()
        {
            // Arrange
            var payments = new List<Payment> { new Payment { PaymentId = 1, PaymentAmount = 100.0 }, new Payment { PaymentId = 2, PaymentAmount = 150.0 } };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(payments);

            // Act
            var result = await _paymentService.GetPaymentHistoryList();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(payments.Count, result.Count);
            CollectionAssert.AreEquivalent(payments, result);
        }

        [Test]
        public void GetPaymentHistoryList_ExceptionThrown_ThrowsException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetAsync()).ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _paymentService.GetPaymentHistoryList());
        }

        [Test]
        public async Task GetUserPayments_ValidUserId_ReturnsUserPayments()
        {
            // Arrange
            var userId = 1;
            var payments = new List<Payment>
            {
                new Payment { PaymentId = 1, UserId = userId, PaymentAmount = 100.0 },
                new Payment { PaymentId = 2, UserId = userId, PaymentAmount = 150.0 }
            };

            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(payments);

            // Act
            var result = await _paymentService.GetUserPayments(userId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(p => p.UserId == userId));
            CollectionAssert.AreEquivalent(payments, result);
        }

        [Test]
        public void GetUserPayments_NoPaymentsFound_ThrowsNoSuchReviewException()
        {
            // Arrange
            var userId = 1;
            _mockRepo.Setup(repo => repo.GetAsync()).ReturnsAsync(new List<Payment>());

            // Act & Assert
            Assert.ThrowsAsync<NoSuchReviewException>(async () => await _paymentService.GetUserPayments(userId));
        }

        [Test]
        public void GetUserPayments_ExceptionThrown_ThrowsException()
        {
            // Arrange
            var userId = 1;
            _mockRepo.Setup(repo => repo.GetAsync()).ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _paymentService.GetUserPayments(userId));
        }

    }

}

