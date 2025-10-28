namespace Stock.API.Core.Contracts.Handler
{
    public interface IMessageHandler
    {
        Task HandleAsync(string messageJson);
    }
}
