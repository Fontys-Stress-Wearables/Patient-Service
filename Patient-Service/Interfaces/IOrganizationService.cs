using Patient_Service.Models;

namespace Patient_Service.Interfaces;

public interface IOrganizationService
{
    bool Exists(string id);
    void Create(Organization organization);
}