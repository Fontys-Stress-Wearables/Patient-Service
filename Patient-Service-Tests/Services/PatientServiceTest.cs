using System;
using Moq;
using Patient_Service.Dtos;
using Patient_Service.Interfaces;
using Patient_Service.Models;
using Patient_Service.Services;
using Xunit;

namespace Patient_Service_Tests.Services;

public class PatientServiceTest
{
    
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<INatsService> _natsServiceMock = new();

    public PatientServiceTest()
    {
        var patient = new Patient();
        
        _unitOfWorkMock.Setup(x => x.Patients.Add(patient)).Returns(patient);
        _unitOfWorkMock.Setup(x => x.Complete()).Returns(0);

        _natsServiceMock.Setup(x => x.Publish("", patient));
    }

    [Fact]
    public void CreatePatient_ShouldSucceed()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object);
        //Act
        var patient = patientService.CreatePatient("id", "John", "Doe", Convert.ToDateTime("Wed Oct 21, 2015"));
        //Assert
        Assert.Equal(patient.FirstName, "John");
        Assert.Equal(patient.LastName, "Doe");
        Assert.Equal(patient.Birthdate, Convert.ToDateTime("Wed Oct 21, 2015"));
    }
}