namespace Sales.API.Core.Entities
{
    public class Sale
    {
        public Guid ID { get; set; }

        public Guid BuyerID { get; set; }

        public Guid ProductID { get; set; }

        public int SellAmount { get; set; }
    }
}
