using Sales.API.Core.Enum;

namespace Sales.API.Service.RabbitMQ.Shared.Models
{
    public class SaleStatusDTO
    {
        public int SaleCode { get; set; }

        public SaleStatus Status { get; set; }
    }
}
