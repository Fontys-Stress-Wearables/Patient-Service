using Patient_Service.Interfaces;
using Patient_Service.Models;

namespace Patient_Service.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public OrganizationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public bool Exists(string id)
    {
        return _unitOfWork.Organizations.GetById(id) != null;
    }

    public void Create(Organization organization)
    {
        _unitOfWork.Organizations.Add(organization);
        _unitOfWork.Complete();
    }
}