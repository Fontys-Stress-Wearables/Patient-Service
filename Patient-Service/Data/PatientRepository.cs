using Patient_Service.Interfaces;
using Patient_Service.Models;

namespace Patient_Service.Data;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(DatabaseContext context) : base(context)
    {
    }
    
    public IEnumerable<Patient> GetAllByTenant(string tenantId)
    {
        return _context.Set<Patient>().Where(x => x.Tenant == tenantId).ToList();
    }

    public Patient GetByIdAndTenant(string tenantId, string patientId)
    {
        return _context.Set<Patient>().Where(x => x.Tenant == tenantId).First(x => x.Id == patientId);
    }

    public Patient UpdatePatient(Patient patient)
    {
        return _context.Set<Patient>().Update(patient).Entity;
    }
}