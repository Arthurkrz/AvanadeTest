namespace Stock.API.Service.RabbitMQ.Shared.Models
{
    public class SaleProcessedDTO : GenericDTO
    {
        public SaleProcessedDTO(int saleCode, int productCode, IList<string> errors)
        {
            SaleCode = saleCode;
            ProductCode = productCode;
            Success = false;
            Errors = errors;
        }

        public SaleProcessedDTO(int saleCode, int productCode)
        {
            SaleCode = saleCode;
            ProductCode = productCode;
            Success = true;
        }

        public int SaleCode { get; set; }

        public int ProductCode { get; set; }

        public bool Success { get; set; }

        public IList<string> Errors { get; set; } = [];
    }
}
