using Sales.API.Core.Enum;

namespace Sales.API.Core.Entities
{
    public class Sale
    {
        public Sale(Guid id, Guid buyerId, Guid productId, int sellAmount, SaleStatus status)
        {
            ID = id;
            BuyerID = buyerId;
            ProductID = productId;
            SellAmount = sellAmount;
            Status = status;
        }

        public Guid ID { get; set; }

        public Guid BuyerID { get; set; }

        public Guid ProductID { get; set; }

        public int SellAmount { get; set; }

        public SaleStatus Status { get; set; }
    }
}
