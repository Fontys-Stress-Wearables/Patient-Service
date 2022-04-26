using Patient_Service.Interfaces;

namespace Patient_Service.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _context;

    public UnitOfWork(DatabaseContext context)
    {
        _context = context;
        Patients = new PatientRepository(_context);
        Organizations = new OrganizationRepository(_context);
    }

    public IOrganizationRepository Organizations { get; }
    public IPatientRepository Patients { get; }
    
    public void Dispose()
    {
        _context.Dispose();
    }
    
    public int Complete()
    {
        return _context.SaveChanges();
    }
}