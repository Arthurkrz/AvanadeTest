using Sales.API.Core.Enum;

namespace Sales.API.Core.Entities
{
    public class Sale
    {
        public Sale(int buyerCPF, int productCode, int sellAmount, SaleStatus status)
        {
            BuyerCPF = buyerCPF;
            ProductCode = productCode;
            SellAmount = sellAmount;
            Status = status;
        }

        public Guid ID { get; } = Guid.NewGuid();

        public int SaleCode { get; set; }

        public int BuyerCPF { get; set; }

        public int ProductCode { get; set; }

        public int SellAmount { get; set; }

        public SaleStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
