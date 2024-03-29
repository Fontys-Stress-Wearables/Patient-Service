﻿using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Patient_Service.Dtos;
using Patient_Service.Exceptions;
using Patient_Service.Interfaces;

namespace Patient_Service.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("patients")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IMapper _mapper;

    public PatientController
    (
        IPatientService patientService, IMapper mapper)
    {
        _patientService = patientService;
        _mapper = mapper;
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
 
        return _mapper.Map<PatientDTO>(patientData);
    }
    
    [HttpPut("{id}")]
    public PatientDTO UpdatePatient(string id, UpdatePatientDto patient)
    {
        var patientData = _patientService.UpdatePatient(HttpContext.User.GetTenantId()!, id, patient.FirstName, patient.LastName, patient.Birthdate);

        return _mapper.Map<PatientDTO>(patientData);
    }
    
    [HttpPost("{id}/profile-image")]
    public async Task<PatientDTO> AddProfileImagePatient(string id, [FromForm] IFormFile image)
    {
        var patientData = await _patientService.AddProfileImagePatient(HttpContext.User.GetTenantId()!, id, image);

        return _mapper.Map<PatientDTO>(patientData);
    }
    
    [HttpDelete("{id}/profile-image")]
    public void RemoveProfileImagePatient(string id)
    {
        _patientService.RemoveProfileImagePatient(HttpContext.User.GetTenantId()!, id);
    }
    
    [HttpPut("{id}/profile-image")]
    public async Task<PatientDTO> UpdateProfileImagePatient(string id, [FromForm] IFormFile image)
    {
        var patientData = await _patientService.UpdateProfileImagePatient(HttpContext.User.GetTenantId()!, id, image);

        return _mapper.Map<PatientDTO>(patientData);
    }
}