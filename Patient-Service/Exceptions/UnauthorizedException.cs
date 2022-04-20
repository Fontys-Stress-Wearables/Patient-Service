using System.Net;

namespace Patient_Service.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message) { }
}