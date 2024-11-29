using Rebus.Routing.TypeBased;
using Rebus.Serialization.Custom;

namespace RebusExtensions.Messages.MessageType
{
    public class MessageTypeMapperConfigurationBuilder
    {
        private readonly CustomTypeNameConventionBuilder _customeNameBuilder;
        private readonly TypeBasedRouterConfigurationExtensions.TypeBasedRouterConfigurationBuilder _routerBuilder;

        internal MessageTypeMapperConfigurationBuilder(CustomTypeNameConventionBuilder builder,
            TypeBasedRouterConfigurationExtensions.TypeBasedRouterConfigurationBuilder routerBuilder)
        {
            _customeNameBuilder = builder;
            _routerBuilder = routerBuilder;
        }

        public MessageTypeMapperConfigurationBuilder Map<TMessage>(string topic)
        {
            _customeNameBuilder.AddWithCustomName<TMessage>(topic);
            return this;
        }

        public MessageTypeMapperConfigurationBuilder Map(Type messageType, string topic)
        {
            _customeNameBuilder.AddWithCustomName(messageType, topic);
            return this;
        }

        public MessageTypeMapperConfigurationBuilder MapWithRoute<TMessage>(string topic, string destinationEnpoint)
        {
            _customeNameBuilder.AddWithCustomName<TMessage>(topic);
            _routerBuilder.Map<TMessage>(destinationEnpoint);
            return this;
        }

        public MessageTypeMapperConfigurationBuilder MapRoute(Type messageType, string topic, string destinationEnpoint)
        {
            _customeNameBuilder.AddWithCustomName(messageType, topic);
            _routerBuilder.Map(messageType, destinationEnpoint);
            return this;
        }
    }
}
