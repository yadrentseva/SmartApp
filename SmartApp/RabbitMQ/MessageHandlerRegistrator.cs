using Newtonsoft.Json;
using SmartApp.Handlers;

namespace SmartApp.RabbitMQ
{
    public class MessageHandlerRegistrator
    {
        private Dictionary<string, List<Type>> handlers;
        IServiceProvider _serviceProvider;

        public MessageHandlerRegistrator(IServiceProvider serviceProvider)
        {
            handlers = new Dictionary<string, List<Type>>();
            _serviceProvider = serviceProvider; 
        }       

        public void Register(string messagetType, Type handlerType)
        {
            if (!handlers.ContainsKey(messagetType))
            {
                handlers.Add(messagetType, new List<Type>());
            }

            if (handlers[messagetType].Any(s => s == handlerType))
                return;

            handlers[messagetType].Add(handlerType); 
        }

        public async Task Handle(string messageType, string messageBody)
        {
            if (!handlers.ContainsKey(messageType))
                return;

            List<Type> typesHandlers = handlers[messageType];
            foreach (Type type in typesHandlers)
            {
                IMyHandler handler = (IMyHandler)ActivatorUtilities.CreateInstance(_serviceProvider, type);
                if (handler != null)
                {
                    var method = type.GetMethod("Handle");
                    if (method != null)
                    {
                        Type typeParam = method.GetParameters()[0].ParameterType; // а если не один параметр?
                        var obj = JsonConvert.DeserializeObject(messageBody, typeParam);

                        method.Invoke(handler, new[] { obj });
                    }
                }
            } 
        }
    }
}
