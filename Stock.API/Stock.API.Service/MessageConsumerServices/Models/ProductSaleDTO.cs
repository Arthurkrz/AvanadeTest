namespace Stock.API.Service.MessageConsumerServices.Models
{
    public class ProductSaleDTO
    {
        public int ProductCode { get; set; }

        public int SoldAmount { get; set; }

        public DateTime SoldAt { get; set; } = DateTime.UtcNow;
    }
}
