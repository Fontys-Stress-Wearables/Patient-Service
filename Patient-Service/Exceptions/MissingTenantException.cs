using System.Net;

namespace Patient_Service.Exceptions;

public class MissingTenantException : AppException 
{
    public MissingTenantException(string message) : base(HttpStatusCode.BadRequest, message) { }
}
