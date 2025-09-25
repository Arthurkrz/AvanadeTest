namespace Stock.API.Core.Entities
{
    public class User : Entity
    {
        // + login fail count, + lockout date, + is active.
        public DateTime LastLoginDate { get; set; } = DateTime.Now;
    }
}
