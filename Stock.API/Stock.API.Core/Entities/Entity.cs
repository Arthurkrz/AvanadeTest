namespace Stock.API.Core.Entities
{
    public class Entity
    {
        public Guid ID { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastModifiedDate { get; set; }
    }
}
