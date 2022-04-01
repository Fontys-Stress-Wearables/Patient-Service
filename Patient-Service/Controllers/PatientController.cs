using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Patient_Service.Interfaces;

namespace Patient_Service.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
}