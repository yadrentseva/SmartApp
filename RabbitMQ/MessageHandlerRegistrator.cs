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

        public async Task Handle(string messageType, string body)
        {
            if (handlers.ContainsKey(messageType))
            {
                List<Type> typesHandlers = handlers[messageType];
                foreach (Type type in typesHandlers)
                {
                    //using (var scope = _serviceProvider.CreateScope())
                    //{
                    //    scope.ServiceProvider.GetService(type);

                    //var aaa = new object[1];

                        IMyHandler handler = (IMyHandler)ActivatorUtilities.CreateInstance(_serviceProvider, type);
                        if (handler != null)
                        {
                            await handler.Handle(body);
                        }
                    //}
                }   
            }
        }
    }
}
