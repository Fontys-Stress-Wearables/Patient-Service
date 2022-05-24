using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Patient_Service.Dtos;
using Patient_Service.Exceptions;
using Patient_Service.Interfaces;

namespace Patient_Service.Controllers;

[Authorize("p-organization-admin")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("patients")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IMapper _mapper;
    private readonly INatsService _natsService;

    public PatientController
    (
        IPatientService patientService, IMapper mapper, INatsService natsService)
    {
        _patientService = patientService;
        _mapper = mapper;
        _natsService = natsService;
    }
    
    [HttpGet]
    public IEnumerable<PatientDTO> GetPatients()
    {
        var patients = _patientService.GetAll(HttpContext.User.GetTenantId()!);

        return _mapper.Map<IEnumerable<PatientDTO>>(patients);
    }
    

    [HttpGet("{id}")]
    public PatientDTO GetPatient(string id)
    {
        var patient = _patientService.GetPatient(HttpContext.User.GetTenantId()!, id);

        return _mapper.Map<PatientDTO>(patient);
    }
    
    [HttpPost]
    public PatientDTO PostPatient(CreatePatientDTO patient)
    {
        var patientData = _patientService.CreatePatient(HttpContext.User.GetTenantId()!, patient.FirstName, patient.LastName, patient.Birthdate);
        _natsService.Publish("patient-created",patientData);
        return _mapper.Map<PatientDTO>(patientData);
    }
    
    [HttpPut("{id}")]
    public PatientDTO UpdatePatient(string id, UpdatePatientDto patient)
    {
        var patientData = _patientService.UpdatePatient(HttpContext.User.GetTenantId()!, id, patient.FirstName, patient.LastName, patient.Birthdate);
        _natsService.Publish("patient-updated",patientData);
        return _mapper.Map<PatientDTO>(patientData);
    }
}