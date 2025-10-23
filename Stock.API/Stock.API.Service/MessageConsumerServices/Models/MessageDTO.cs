namespace Stock.API.Service.MessageConsumerServices.Models
{
    public class MessageDTO
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
