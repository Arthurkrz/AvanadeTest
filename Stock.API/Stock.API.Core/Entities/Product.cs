namespace Stock.API.Core.Entities
{
    public class Product : Entity
    {
        public Product(string name, string description, decimal price, int amountInStock)
        {
            Name = name;
            Description = description;
            Price = price;
            AmountInStock = amountInStock;
        }

        public int Code { get; set; }

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;
        
        public decimal Price { get; set; }
        
        public int AmountInStock { get; set; }
    }
}
