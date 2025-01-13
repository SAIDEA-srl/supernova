namespace OrangeButton.Models;


public class Message<T>
{
    public DateTimeOffset DateTime { get; set; }

    public string? Source { get; set; }

    public T? Data { get; set; }

}


public enum ExectutionStatus
{
    Created = 0,
    Execution = 1,
    End = 2,
}


public class GatewayExectutionStatus
{
    public Guid HookId { get; set; }
    public Guid ExecutionId { get; set; }
    public ExectutionStatus Status { get; set; }
}
