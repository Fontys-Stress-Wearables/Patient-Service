namespace Patient_Service.Models;

public class NatsMessage<T>
{
    public string origin { get; set; } = "patient-service";
    public string target { get; set; }
    public T message { get; set; }
}