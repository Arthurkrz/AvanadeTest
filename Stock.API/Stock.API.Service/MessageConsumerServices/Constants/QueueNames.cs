namespace Stock.API.Service.MessageConsumerServices.Constants
{
    public class QueueNames
    {
        public const string MainQueue = "main-queue";
        public const string RetryQueue = "retry-queue";
        public const string DeadLetterQueue = "dead-letter-queue";
    }
}
