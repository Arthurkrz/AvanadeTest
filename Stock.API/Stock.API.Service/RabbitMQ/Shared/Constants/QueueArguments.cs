namespace Stock.API.Service.RabbitMQ.Shared.Constants
{
    public class QueueArguments
    {
        public const string MessageTitle = "x-message-title";
        public const string DeadLetterExchange = "x-dead-letter-exchange";
        public const string DeadLetterRoutingKey = "x-dead-letter-routing-key";
        public const string MaxLength = "x-max-length";
        public const string MaxBytesLength = "x-max-length-bytes";
        public const string QueueLifeTime = "x-expires";
        public const string Overflow = "x-overflow";
        public const string MaxPriority = "x-max-priority";
        public const string QueueMode = "x-queue-mode";
        public const string AutoDelete = "x-auto-delete";
    }
}
