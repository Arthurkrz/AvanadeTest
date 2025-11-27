namespace Identity.API.Core.Entities
{
    public class Entity
    {
        public Guid ID { get; set; } = Guid.NewGuid();

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

        public DateTime LastLoginDate { get; set; } = DateTime.Now;

        public int FailedLoginCount { get; set; } = 0;

        public DateTime? LockoutEnd { get; set; } = null;
    }
}
