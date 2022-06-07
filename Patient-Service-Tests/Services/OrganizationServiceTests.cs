using System;
using Moq;
using Patient_Service.Interfaces;
using Patient_Service.Models;
using Patient_Service.Services;
using Xunit;

namespace Patient_Service_Tests.Services;

public class OrganizationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    
    [Fact]
    public void Exists_ShouldSucceed()
    {
        //Arrange
        IOrganizationService organizationService = new OrganizationService(_unitOfWorkMock.Object);
        var organization = new Organization() {id = "id"};
        _unitOfWorkMock.Setup(x => x.Organizations.GetById("id")).Returns(organization);
        //Act
        var result = organizationService.Exists("id");
        //Assert
        Assert.NotNull(result);
        Assert.Equal(result, true);
    }

    [Fact]
    public void Create_ShouldSucceed()
    {
        //Arrange
        IOrganizationService organizationService = new OrganizationService(_unitOfWorkMock.Object);
        var organization = new Organization() {id = "id"};
        _unitOfWorkMock.Setup(x => x.Organizations.Add(organization)).Returns(organization);
        //Act
        organizationService.Create(organization);
        //Assert
        _unitOfWorkMock.Verify(x => x.Complete(), Times.Once);
    }
}