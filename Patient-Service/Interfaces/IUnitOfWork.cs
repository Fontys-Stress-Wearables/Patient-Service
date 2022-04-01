namespace Patient_Service.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    int Complete();
}