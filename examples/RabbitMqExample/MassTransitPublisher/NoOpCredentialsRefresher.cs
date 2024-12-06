using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransitPublisher
{
    internal class NoOpCredentialsRefresher : ICredentialsRefresher
    {
        public ICredentialsProvider Register(ICredentialsProvider provider, NotifyCredentialRefreshed callback)
        {
            return provider;
        }

        public bool Unregister(ICredentialsProvider provider)
        {
            return false;
        }
    }
}
