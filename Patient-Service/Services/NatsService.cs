using System.Text;
using NATS.Client;
using Newtonsoft.Json;
using Patient_Service.Interfaces;
using Patient_Service.Models;

namespace Patient_Service.Services;

public class NatsService : INatsService
{
    private readonly IConfiguration _configuration;
    private readonly IConnection? _connection;
    private IAsyncSubscription? _asyncSubscription;

    public NatsService(IConfiguration configuration)
    {
        _configuration = configuration;
        _connection = Connect();
    }

    public IConnection Connect()
    {
        ConnectionFactory cf = new ConnectionFactory();
        Options opts = ConnectionFactory.GetDefaultOptions();

        opts.Url = _configuration.GetConnectionString("NATSContext");

        return cf.CreateConnection(opts);
    }

    public void Publish<T>(string target, T data)
    {
        var message = new NatsMessage<T>{target = target, message = data};
        _connection?.Publish(target, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
    }

    public void Subscribe(string target, EventHandler<MsgHandlerEventArgs> handler)
    {
        _asyncSubscription = _connection?.SubscribeAsync(target);
        if (_asyncSubscription != null)
        {
            _asyncSubscription.MessageHandler += handler;
            _asyncSubscription.Start();
        }
    }
}