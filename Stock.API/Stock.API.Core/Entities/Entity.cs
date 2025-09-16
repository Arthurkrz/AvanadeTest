namespace Stock.API.Core.Entities
{
    public class Entity
    {
        public Guid ID { get; set; } = Guid.NewGuid();

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    }
}
