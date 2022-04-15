using Patient_Service.Models;

namespace Patient_Service.Interfaces;

public interface IPatientService
{
    public Patient CreatePatient(string tenantId, string firstName, string lastName, DateTime birthdate);

    public IEnumerable<Patient> GetAll(string tenantId);

    public Patient GetPatient(string tenantId, string id);

    public Patient UpdatePatient(string tenantId, string patientId, string? firstName, string? lastName,
        DateTime? birthdate);
}