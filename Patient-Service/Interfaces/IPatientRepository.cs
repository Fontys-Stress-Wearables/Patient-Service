using Patient_Service.Models;

namespace Patient_Service.Interfaces;

public interface IPatientRepository : IGenericRepository<Patient>
{
    public IEnumerable<Patient> GetAllByTenant(string tenantId);
    public Patient GetByIdAndTenant(string tenantId, string patientId);
    public Patient UpdatePatient(Patient patient);
}