using Patient_Service.Interfaces;
using Patient_Service.Models;

namespace Patient_Service.Data;

public class OrganizationRepository : GenericRepository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(DatabaseContext context) : base(context)
    {
    }
}