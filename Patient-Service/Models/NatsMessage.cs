namespace Patient_Service.Models;

public class NatsMessage<T>
{
    public string origin { get; set; } = "organization-service";
    public string target { get; set; }
    public T message { get; set; }
}