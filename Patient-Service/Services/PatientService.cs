using Patient_Service.Exceptions;
using Patient_Service.Interfaces;
using Patient_Service.Models;

namespace Patient_Service.Services;

public class PatientService : IPatientService
{
    private readonly IUnitOfWork _unitOfWork;

    public PatientService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Patient CreatePatient(string firstName, string lastName, DateTime birthdate)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new BadRequestException("First name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new BadRequestException("Last name cannot be empty.");
        }

        if (birthdate > DateTime.Now)
        {
            throw new BadRequestException("Birthdate cannot be after the current date.");
        }

        var patient = new Patient()
        {
            FirstName = firstName,
            LastName = lastName,
            Birthdate = birthdate,
            IsActive = true,
            Id = Guid.NewGuid().ToString()
        };

        _unitOfWork.Patients.Add(patient);
        _unitOfWork.Complete();

        return patient;
    }

    public IEnumerable<Patient> GetAll()
    {
        return _unitOfWork.Patients.GetAll();
    }

    public Patient GetPatient(string id)
    {
        var patient = _unitOfWork.Patients.GetById(id);

        if (patient == null)
        {
            throw new NotFoundException($"Patient with id '{id}' doesn't exist.");
        }

        return patient;
    }
}