namespace Models
{
    public class Message<T>
    {
        public DateTimeOffset DateTime { get; set; }

        public string? Source { get; set; }

        public T? Data { get; set; }
    }
}
