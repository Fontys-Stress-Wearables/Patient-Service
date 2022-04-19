namespace Patient_Service.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IOrganizationRepository Organizations { get; }
    IPatientRepository Patients { get; }
    int Complete();
}