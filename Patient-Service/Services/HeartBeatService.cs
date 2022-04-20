using Patient_Service.Interfaces;

namespace Patient_Service.Services;

public class HeartBeatService : BackgroundService
{
    private readonly INatsService _natsService;

    public HeartBeatService(INatsService natsService)
    {
        _natsService = natsService;
    }

    private void HeartbeatTimerCallback(object? state)
    {
        _natsService.Publish("technical_health", "heartbeat");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new Timer(HeartbeatTimerCallback, null, 1000, 30000);
        return Task.CompletedTask;
    }
}