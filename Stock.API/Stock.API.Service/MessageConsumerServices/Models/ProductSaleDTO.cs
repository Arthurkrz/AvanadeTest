namespace Stock.API.Service.MessageConsumerServices.Models
{
    public class ProductSaleDTO
    {
        public Guid ID { get; set; } = Guid.NewGuid();

        public Guid ProductID { get; set; }

        public int SoldAmount { get; set; }

        public DateTime SoldAt { get; set; } = DateTime.UtcNow;
    }
}
