using Patient_Service.Interfaces;
using Patient_Service.Models;

namespace Patient_Service.Data;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(DatabaseContext context) : base(context)
    {
    }
}