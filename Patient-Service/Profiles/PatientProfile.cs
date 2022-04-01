using AutoMapper;
using Patient_Service.Dtos;
using Patient_Service.Models;

namespace Patient_Service.Profiles;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<Patient, PatientDTO>();
    }
}