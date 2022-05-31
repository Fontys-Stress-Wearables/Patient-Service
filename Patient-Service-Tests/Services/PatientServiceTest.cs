using System;
using Moq;
using Patient_Service.Exceptions;
using Patient_Service.Interfaces;
using Patient_Service.Models;
using Patient_Service.Services;
using Xunit;

namespace Patient_Service_Tests.Services;

public class PatientServiceTest
{
    
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<INatsService> _natsServiceMock = new();
    private readonly Mock<IBlobStorageService> _blobStorage = new();

    public PatientServiceTest()
    {
        var patient = new Patient();
        
        _unitOfWorkMock.Setup(x => x.Patients.Add(patient)).Returns(patient);
        _unitOfWorkMock.Setup(x => x.Complete()).Returns(0);
        _unitOfWorkMock.Setup(x => x.Patients.GetByIdAndTenant("tenant", "patient")).Returns(() => null);

        _natsServiceMock.Setup(x => x.Publish("", patient));
    }

    [Fact]
    public void CreatePatient_ShouldSucceed()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        //Act
        var patient = patientService.CreatePatient("id", "John", "Doe", Convert.ToDateTime("Oct 21, 2015"));
        //Assert
        Assert.Equal(patient.FirstName, "John");
        Assert.Equal(patient.LastName, "Doe");
        Assert.Equal(patient.Birthdate, Convert.ToDateTime("Oct 21, 2015"));
    }
    
    [Fact]
    public void CreatePatientWithoutFirstName_ShouldFail()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        //Act
        var exception = Assert.Throws<BadRequestException>(() => 
            patientService.CreatePatient("id", "", "Doe", Convert.ToDateTime("Oct 21, 2015"))
            );
        //Assert
        Assert.Equal("First name cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void CreatePatientWithoutLastName_ShouldFail()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        //Act
        var exception = Assert.Throws<BadRequestException>(() => 
            patientService.CreatePatient("id", "John", "", Convert.ToDateTime("Oct 21, 2015"))
        );
        //Assert
        Assert.Equal("Last name cannot be empty.", exception.Message);
    }
    
    [Fact]
    public void CreatePatientWithDateGreaterThanCurrent_ShouldFail()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        //Act
        var exception = Assert.Throws<BadRequestException>(() => 
            patientService.CreatePatient("id", "John", "Doe", Convert.ToDateTime("Dec 12, 9999"))
        );
        //Assert
        Assert.Equal("Birthdate cannot be after the current date.", exception.Message);
    }

    [Fact]
    public void GetPatient_ShouldFail()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        //Act
        var exception = Assert.Throws<NotFoundException>(() => 
            patientService.GetPatient("id", "id")
        );
        //Assert
        Assert.Equal("Patient with id 'id' doesn't exist.", exception.Message);
    }
}