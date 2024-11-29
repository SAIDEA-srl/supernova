using Models;
using Models.OrangeButton;
using Newtonsoft.Json;
using Rebus.Handlers;

namespace Subscriber.Handlers
{
    public class OMIssueHandler : IHandleMessages<Message<OMIssue>>
    {
        public async System.Threading.Tasks.Task Handle(Message<OMIssue> message)
        {
            await System.Threading.Tasks.Task.CompletedTask;

            Console.WriteLine("New OMIssue message received");
            Console.WriteLine(JsonConvert.SerializeObject(message));
        }
    }
}
