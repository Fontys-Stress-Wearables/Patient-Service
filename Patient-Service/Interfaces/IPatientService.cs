using Patient_Service.Models;

namespace Patient_Service.Interfaces;

public interface IPatientService
{
    public Patient CreatePatient(string firstName, string lastName, DateTime birthdate);

    public IEnumerable<Patient> GetAll();

    public Patient GetPatient(string id);
}