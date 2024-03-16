namespace SmartApp.Models
{
    public class SmartConfig
    {
        public SmartDBConnection smartDBConnection { get; set; }

        public RabbitMQSettings rabbitMQSettings { get; set; }

    }
}
