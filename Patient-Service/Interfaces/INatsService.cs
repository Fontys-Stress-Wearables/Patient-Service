using NATS.Client;
using Patient_Service.Models;

namespace Patient_Service.Interfaces;

public interface INatsService
{
    public IConnection Connect();
    public void Publish<T>(string topic, T data);
    public void Subscribe<T>(string target, Action<NatsMessage<T>> handler);
}