namespace Sales.API.Service.RabbitMQ.Shared.Models
{
    public class ProductSaleDTO : GenericDTO
    {
        public int SaleCode { get; set; }

        public int ProductCode { get; set; }

        public int SoldAmount { get; set; }
    }
}
