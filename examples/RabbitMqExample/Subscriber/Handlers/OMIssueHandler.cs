using Models;
using Models.OrangeButton;
using Newtonsoft.Json;

namespace Subscriber.Handlers
{
    public class OMIssueHandler
    {
        public static void Handle(string message)
        {
            Console.WriteLine("New OMIssue message received");
            Console.WriteLine(message);

            var m = JsonConvert.DeserializeObject<Message<OMIssue>>(message);
        }
    }
}
