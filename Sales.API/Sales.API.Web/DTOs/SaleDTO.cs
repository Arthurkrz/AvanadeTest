namespace Sales.API.Web.DTOs
{
    public class SaleDTO
    {
        public Guid ID { get; set; }

        public Guid BuyerID { get; set; }

        public Guid ProductID { get; set; }

        public int SellAmount { get; set; }
    }
}
