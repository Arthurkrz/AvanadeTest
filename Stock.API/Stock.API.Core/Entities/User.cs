namespace Stock.API.Core.Entities
{
    public class User : Entity
    {
        public DateTime LastLoginDate { get; set; } = DateTime.Now;
        
        public int FailedLoginCount { get; set; } = 0;

        public DateTime? LockoutEnd { get; set; } = null;
    }
}
