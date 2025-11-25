namespace Sales.API.Service.RabbitMQ.Shared.Models
{
    public class SaleStatusDTO : GenericDTO
    {
        public int SaleCode { get; set; }

        public int ProductCode { get; set; }

        public bool Success { get; set; }

        public IList<string> Errors { get; set; } = [];
    }
}
