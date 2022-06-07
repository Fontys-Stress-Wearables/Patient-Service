using Patient_Service.Exceptions;
using Patient_Service.Interfaces;
using Patient_Service.Models;

namespace Patient_Service.Services;

public class PatientService : IPatientService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INatsService _natsService;
    private readonly IBlobStorageService _blobStorageService;

    public PatientService(IUnitOfWork unitOfWork, INatsService natsService, IBlobStorageService blobStorageService)
    {
        _unitOfWork = unitOfWork;
        _natsService = natsService;
        _blobStorageService = blobStorageService;
    }

    public Patient CreatePatient(string tenantId, string firstName, string lastName, DateTime birthdate)
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
            Tenant = tenantId,
            FirstName = firstName,
            LastName = lastName,
            Birthdate = birthdate,
            IsActive = true,
            Id = Guid.NewGuid().ToString()
        };

        _unitOfWork.Patients.Add(patient);
        _natsService.Publish("patient-created",patient.Tenant,patient);
            
        _unitOfWork.Complete();

        return patient;
    }

    public IEnumerable<Patient> GetAll(string tenant)
    {
        return _unitOfWork.Patients.GetAllByTenant(tenant);
    }

    public Patient GetPatient(string tenantId, string id)
    {
        var patient = _unitOfWork.Patients.GetByIdAndTenant(tenantId,id);

        if (patient == null)
        {
            throw new NotFoundException($"Patient with id '{id}' doesn't exist.");
        }

        return patient;
    }

    public Patient UpdatePatient(string tenantId, string patientId, string? firstName, string? lastName, DateTime? birthdate)
    {
        var patient = GetPatient(tenantId, patientId);

        if (birthdate != null && birthdate > DateTime.Now)
        {
            throw new BadRequestException("Birthdate cannot be after the current date.");
        }
        
        patient.FirstName = firstName ?? patient.FirstName;
        patient.LastName = lastName ?? patient.LastName;
        patient.Birthdate = birthdate ?? patient.Birthdate;

        _unitOfWork.Patients.UpdatePatient(patient);
        _natsService.Publish("patient-updated",patient.Tenant,patient);
        
        _unitOfWork.Complete();
        
        return patient;
    }

    public async Task<Patient> AddProfileImagePatient(string tenantId, string patientId, IFormFile image)
    {
        var patient = GetPatient(tenantId, patientId);

        var fileName = $"{Guid.NewGuid()}.jpg";
        
        // var imageBlobUrl = await _blobStorageService.UploadProfileImage_GetImageUrl(image, fileName);
        
        // patient.ProfileImageUrl = imageBlobUrl;
        // patient.ProfileImageName = fileName;
        _unitOfWork.Patients.UpdatePatient(patient);
        _natsService.Publish("patient-profileImage-added",patient.Tenant,patient);

        _unitOfWork.Complete();

        return patient;
    }
    
    public void RemoveProfileImagePatient(string tenantId, string patientId)
    {
        var patient = GetPatient(tenantId, patientId);
        
        // _blobStorageService.DeleteProfileImage(patient.ProfileImageName);
        
        patient.ProfileImageUrl = "";
        patient.ProfileImageName = "";
        _unitOfWork.Patients.UpdatePatient(patient);
        _natsService.Publish("patient-profileImage-removed",patient.Tenant, patient);

        _unitOfWork.Complete();
    }
    
    public async Task<Patient> UpdateProfileImagePatient(string tenantId, string patientId, IFormFile image)
    {
        var patient = GetPatient(tenantId, patientId);
        
        // _blobStorageService.DeleteProfileImage(patient.ProfileImageName);
        
        var fileName = $"{Guid.NewGuid()}.jpg";
        
        // var imageBlobUrl = await _blobStorageService.UploadProfileImage_GetImageUrl(image, fileName);
        
        // patient.ProfileImageUrl = imageBlobUrl;
        // patient.ProfileImageName = image.FileName;
        _unitOfWork.Patients.UpdatePatient(patient);
        _natsService.Publish("patient-profileImage-changed",patient.Tenant, patient);

        _unitOfWork.Complete();

        return patient;
    }
}