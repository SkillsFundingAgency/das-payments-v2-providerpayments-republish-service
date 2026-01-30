
namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.Models
{
    public class ResubmitCompletionPaymentMessagesRequest
    {
        public string BlobStorageContainerName { get; set; }
        public string FileName { get; set; }
    }
}
