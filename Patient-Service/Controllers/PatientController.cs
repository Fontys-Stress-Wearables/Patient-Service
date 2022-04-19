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

    public PatientController
    (
        IPatientService patientService, IMapper mapper
    )
    {
        _patientService = patientService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public IEnumerable<PatientDTO> GetPatients()
    {
        var tenantId = HttpContext.User.GetTenantId();
        
        var patients = _patientService.GetAll(tenantId ?? throw new MissingTenantException("Could not get tenant from user"));

        return _mapper.Map<IEnumerable<PatientDTO>>(patients);
    }
    

    [HttpGet("{id}")]
    public PatientDTO GetPatient(string id)
    {
        var tenantId = HttpContext.User.GetTenantId();
        
        var patient = _patientService.GetPatient(tenantId ?? throw new MissingTenantException("Could not get tenant from user"), id);

        return _mapper.Map<PatientDTO>(patient);
    }
    
    [HttpPost]
    public PatientDTO PostPatient(CreatePatientDTO patient)
    {
        var tenantId = HttpContext.User.GetTenantId();

        var patientData = _patientService.CreatePatient(tenantId ?? throw new MissingTenantException("Could not get tenant from user"), patient.FirstName, patient.LastName, patient.Birthdate);

        return _mapper.Map<PatientDTO>(patientData);
    }
    
    [HttpPut("{id}")]
    public PatientDTO UpdatePatient(string id, UpdatePatientDto patient)
    {
        var tenantId = HttpContext.User.GetTenantId();

        var patientData = _patientService.UpdatePatient(tenantId ?? throw new MissingTenantException("Could not get tenant from user"), id, patient.FirstName, patient.LastName, patient.Birthdate);

        return _mapper.Map<PatientDTO>(patientData);
    }
}