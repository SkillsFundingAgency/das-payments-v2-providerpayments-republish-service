using System;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.Models
{
    public class ServiceBusMessage
    {
        public string Body { get; set; }
        public string ContentType { get; set; }
        public string CorrelationId { get; set; }
        public object DeadLetterSource { get; set; }
        public int DeliveryCount { get; set; }
        public int EnqueuedSequenceNumber { get; set; }
        public DateTime EnqueuedTimeUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public bool ForcePersistence { get; set; }
        public bool IsBodyConsumed { get; set; }
        public object Label { get; set; }
        public object LockedUntilUtc { get; set; }
        public object LockToken { get; set; }
        public string MessageId { get; set; }
        public object PartitionKey { get; set; }
        public object Properties { get; set; }
        public string ReplyTo { get; set; }
        public object ReplyToSessionId { get; set; }
        public DateTime ScheduledEnqueueTimeUtc { get; set; }
        public int SequenceNumber { get; set; }
        public object SessionId { get; set; }
        public int Size { get; set; }
        public int State { get; set; }
        public string TimeToLive { get; set; }
        public object To { get; set; }
        public object ViaPartitionKey { get; set; }
    }

}

