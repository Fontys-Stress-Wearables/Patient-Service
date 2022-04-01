using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Patient_Service.Dtos;
using Patient_Service.Interfaces;

namespace Patient_Service.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly INatsService _natsService;
    private readonly IMapper _mapper;

    public PatientController
    (
        IPatientService patientService, INatsService natsService, IMapper mapper
    )
    {
        _patientService = patientService;
        _natsService = natsService;
        _mapper = mapper;
    }
    
    [HttpGet("patients")]
    public IEnumerable<PatientDTO> GetPatients()
    {
        var patients = _patientService.GetAll();

        return _mapper.Map<IEnumerable<PatientDTO>>(patients);
    }
    

    [HttpGet("patients/{id}")]
    public PatientDTO GetPatient(string id)
    {
        var patient = _patientService.GetPatient(id);

        return _mapper.Map<PatientDTO>(patient);
    }
    
    [HttpPost("patients")]
    public PatientDTO PostPatient(CreatePatientDTO patient)
    {
        var patientData = _patientService.CreatePatient(patient.FirstName, patient.LastName, patient.Birthdate);
        _natsService.Publish("technical_health", "hearthbeat");

        return _mapper.Map<PatientDTO>(patientData);
    }
}