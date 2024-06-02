using RoadReady.Models;

namespace RoadReady.Interface
{
    public interface IPaymentAdminService
    {
        public Task<List<Payment>> GetPaymentHistoryList();
        public Task<List<Payment>> GetPendingPayments();
        public Task<Payment> UpdatePaymentStatus(int paymentId, string paymentsStatus);
        public Task<List<Payment>> GetUserPayments(int userId);
    }
}
