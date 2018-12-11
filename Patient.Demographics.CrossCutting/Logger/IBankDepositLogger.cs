using System.Threading.Tasks;

namespace Patient.Demographics.CrossCutting.Logger
{
    public interface IBankDepositLogger
    {
        Task LogAsync(int pointsTransactionId, string customerId, string countryCode, string bankAccountNumber, long trackingId, string response);
    }
}