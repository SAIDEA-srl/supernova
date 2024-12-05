using RabbitMQ.Client;
using Rebus.Logging;
using Rebus.Messages;
using Rebus.RabbitMq;
using Rebus.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RebusSubscriber.Extensions
{
    public class CustomRabbitMqTransport : ITransport
    {
        private readonly RabbitMqTransport realTransport;

        public CustomRabbitMqTransport(RabbitMqTransport realTransport) 
        {
            this.realTransport = realTransport;
        }

        public string Address => realTransport.Address;

        public void CreateQueue(string address)
        {
            realTransport.CreateQueue(address);
        }

        public async Task<TransportMessage> Receive(ITransactionContext context, CancellationToken cancellationToken)
        {
            var message = await realTransport.Receive(context, cancellationToken);
            return message;
        }

        public Task Send(string destinationAddress, TransportMessage message, ITransactionContext context)
        {
            return realTransport.Send(destinationAddress, message, context);
        }
    }
}
