using Rebus.Serialization;
using Rebus.Topic;

namespace Subscriber
{
    public class TopicNameConvention : ITopicNameConvention
    {
        private IMessageTypeNameConvention _messageTypeNameConvention;

        public TopicNameConvention(IMessageTypeNameConvention messageTypeNameConvention)
        {
            _messageTypeNameConvention = messageTypeNameConvention;
        }

        public string GetTopic(Type eventType)
        {
            return _messageTypeNameConvention.GetTypeName(eventType);
        }
    }
}
