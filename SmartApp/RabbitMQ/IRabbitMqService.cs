namespace SmartApp.RabbitMQ
{
    public interface IRabbitMqService
    {
        void SendMessage(string typeMessage, object obj);
        void SendMessage(string typeMessage, string message);
    }
}
