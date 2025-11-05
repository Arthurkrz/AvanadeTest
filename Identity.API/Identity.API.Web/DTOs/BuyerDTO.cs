namespace Identity.API.Web.DTOs
{
    public class BuyerDTO
    {
        public string? Username { get; set; }
        
        public string? Name { get; set; }
        
        public string? CPF { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }
        
        public string? PhoneNumber { get; set; }
        
        public string? DeliveryAddress { get; set; }
    }
}
