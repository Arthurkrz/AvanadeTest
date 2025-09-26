namespace Stock.API.Web.DTOs
{
    public class ProductDTO
    {
        public string? Name { get; set; } = default!;

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public int? AmountInStock { get; set; }
    }
}
