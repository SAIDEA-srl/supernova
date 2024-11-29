using Newtonsoft.Json;
using Rebus.Config;
using Rebus.Routing;
using Rebus.Routing.TypeBased;
using Rebus.Serialization;
using Rebus.Serialization.Custom;
using Rebus.Serialization.Json;
using Rebus.Topic;
using RebusExtensions.Messages.MessageType;
using RebusExtensions.Topic;

namespace RebusExtensions
{
    public static class RebusConfigurerExtensions
    {
        public static RebusConfigurer MapTypes(this RebusConfigurer configurerInstance, Action<MessageTypeMapperConfigurationBuilder> configurer)
        {
            if (configurer == null) throw new ArgumentNullException(nameof(configurer));

            configurerInstance.Serialization((s) =>
            {
                s.UseNewtonsoftJson(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None
                });

                s.OtherService<ITopicNameConvention>().Register(c =>
                {
                    var messageTypeNameConvention = c.Get<IMessageTypeNameConvention>();
                    return new TopicNameConvention(messageTypeNameConvention);
                });

                var builder = s.UseCustomMessageTypeNames();
                var routerBuilder = s.OtherService<IRouter>().TypeBased();
                configurer(new MessageTypeMapperConfigurationBuilder(builder, routerBuilder));
            });

            return configurerInstance;
        }
    }
}
