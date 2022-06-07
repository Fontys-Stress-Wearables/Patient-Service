using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
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
        
        _unitOfWorkMock.Setup(x => x.Patients.Add(patient)).Returns(() => null);
        _unitOfWorkMock.Setup(x => x.Complete()).Returns(0);
    }

    [Fact]
    public void CreatePatient_ShouldSucceed()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        //Act
        var patient = patientService.CreatePatient("id", "John", "Doe", Convert.ToDateTime("Oct 21, 2015"));
        //Assert
        _unitOfWorkMock.Verify(x => x.Complete(), Times.Once);
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
    public void GetAll_ShouldSucceed()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        var patientList = new List<Patient>()
        {
            new Patient() { Id = "patientId1"},
            new Patient() { Id = "patientId2"}
        };
        _unitOfWorkMock.Setup(x => x.Patients.GetAllByTenant("")).Returns(patientList);
        //Act
        var result = patientService.GetAll("");
        //Assert
        _unitOfWorkMock.Verify(x => x.Patients.GetAllByTenant(""));
        Assert.NotNull(result);
        Assert.Equal(patientList.Count, result.Count());
    }

    [Fact]
    public void GetPatient_ShouldSucceed()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        var patient = new Patient() { Id = "patientId"};
        _unitOfWorkMock.Setup(x => x.Patients.GetByIdAndTenant("", patient.Id)).Returns(patient);
        //Act
        var result = patientService.GetPatient("", patient.Id);
        //Assert
        _unitOfWorkMock.Verify(x => x.Patients.GetByIdAndTenant("", patient.Id));
        Assert.NotNull(result);
        Assert.Equal(patient.Id, result.Id);
    }

    [Fact]
    public void GetPatient_ShouldFail()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        _unitOfWorkMock.Setup(x => x.Patients.GetByIdAndTenant("id", "id")).Returns(() => null);
        //Act
        var exception = Assert.Throws<NotFoundException>(() => 
            patientService.GetPatient("id", "id")
        );
        //Assert
        Assert.Equal("Patient with id 'id' doesn't exist.", exception.Message);
    }
    
    [Fact]
    public void UpdatePatient_ShouldSucceed()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        var patient = new Patient() { Id = "patientId"};
        _unitOfWorkMock.Setup(x => x.Patients.GetByIdAndTenant("", patient.Id)).Returns(patient);
        //Act
        var result = patientService.UpdatePatient("", patient.Id, "firstname", "lastname", Convert.ToDateTime("Dec 12, 1999"));
        //Assert
        _unitOfWorkMock.Verify(x => x.Complete());
        Assert.NotNull(result);
        Assert.Equal(patient.Id, result.Id);
    }
    
    [Fact]
    public void UpdatePatientWithDateGreaterThanCurrent_ShouldFail()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        var patient = new Patient() { Id = "patientId"};
        _unitOfWorkMock.Setup(x => x.Patients.GetByIdAndTenant("id", patient.Id)).Returns(patient);
        //Act
        var exception = Assert.Throws<BadRequestException>(() => 
            patientService.UpdatePatient("id", "patientId", "Doe","Bob", Convert.ToDateTime("Dec 12, 9999"))
        );
        //Assert
        Assert.Equal("Birthdate cannot be after the current date.", exception.Message);
    }
    
    [Fact]
    public async void AddProfileImagePatient_ShouldSucceed()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        var patient = new Patient() { Id = "patientId"};
        _unitOfWorkMock.Setup(x => x.Patients.GetByIdAndTenant("", patient.Id)).Returns(patient);
        //Act
        var result = await patientService.AddProfileImagePatient("", patient.Id, 
            new FormFile(File.Open("blue.jpg", FileMode.Open), 0, 0, "", ""));
        //Assert
        _unitOfWorkMock.Verify(x => x.Complete());
        Assert.NotNull(result);
        Assert.Equal(patient.Id, result.Id);
    }
    
    [Fact]
    public void RemoveProfileImagePatient_ShouldSucceed()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        var patient = new Patient() { Id = "patientId"};
        _unitOfWorkMock.Setup(x => x.Patients.GetByIdAndTenant("", patient.Id)).Returns(patient);
        //Act
        patientService.RemoveProfileImagePatient("", patient.Id);
        //Assert
        _unitOfWorkMock.Verify(x => x.Complete());
    }
    
    [Fact]
    public async void UpdateProfileImagePatient_ShouldSucceed()
    {
        //Arrange
        IPatientService patientService = new PatientService(_unitOfWorkMock.Object, _natsServiceMock.Object, _blobStorage.Object);
        var patient = new Patient() { Id = "patientId"};
        _unitOfWorkMock.Setup(x => x.Patients.GetByIdAndTenant("", patient.Id)).Returns(patient);
        //Act
        var result = await patientService.UpdateProfileImagePatient("", patient.Id, 
            new FormFile(File.Open("blue1.jpg", FileMode.Open), 0, 0, "", ""));
        //Assert
        _unitOfWorkMock.Verify(x => x.Complete());
        Assert.NotNull(result);
        Assert.Equal(patient.Id, result.Id);
    }
}