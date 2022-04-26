using Patient_Service.Interfaces;
using Patient_Service.Models;

namespace Patient_Service.Services;

public class NatsSubscriptionService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly INatsService _natsService;
    
    public NatsSubscriptionService(IServiceProvider services, INatsService natsService)
    {
        _services = services;
        _natsService = natsService;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _natsService.Subscribe<Organization>("organization-created", OnOrganizationCreated);
        return Task.CompletedTask;
    }

    private void OnOrganizationCreated(NatsMessage<Organization> message)
    {
        using var scope = _services.CreateScope();
        
        var scopedOrganizationService = 
            scope.ServiceProvider
                .GetRequiredService<IOrganizationService>();
        
        scopedOrganizationService.Create(message.message);
    }
}