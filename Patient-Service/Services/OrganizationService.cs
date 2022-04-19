using System.Text;
using NATS.Client;
using Newtonsoft.Json;
using Patient_Service.Exceptions;
using Patient_Service.Interfaces;
using Patient_Service.Models;

namespace Patient_Service.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrganizationService(IUnitOfWork unitOfWork, INatsService natsService)
    {
        _unitOfWork = unitOfWork;

        natsService.Subscribe("organization-created", OrganizationCreated);
    }
    
    public bool Exists(string id)
    {
        return _unitOfWork.Organizations.GetById(id) != null;
    }

    private void OrganizationCreated(object? sender, MsgHandlerEventArgs e)
    {
        var org = JsonConvert.DeserializeObject<Organization>(Encoding.UTF8.GetString(e.Message.Data));

        if (org == null) return;
        
        _unitOfWork.Organizations.Add(org);
        _unitOfWork.Complete();
    }
}