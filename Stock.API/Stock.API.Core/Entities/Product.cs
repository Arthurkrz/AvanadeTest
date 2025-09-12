namespace Stock.API.Core.Entities
{
    public class Product : Entity
    {
        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;
        
        public double Price { get; set; }
        
        public int AmountInStock { get; set; }
    }
}
