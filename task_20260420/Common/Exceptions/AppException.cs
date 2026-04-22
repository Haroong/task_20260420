using System.Net;
using task_20260420.Common.Models;

namespace task_20260420.Common.Exceptions;

public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string Code { get; }

    protected AppException(HttpStatusCode statusCode, string code, string message)
        : base(message)
    {
        StatusCode = statusCode;
        Code = code;
    }

    protected AppException(HttpStatusCode statusCode, string code, string message, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        Code = code;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(HttpStatusCode.NotFound, ResponseCode.NotFound, message) { }
}

public class InvalidFormatException : AppException
{
    public InvalidFormatException(string message)
        : base(HttpStatusCode.BadRequest, ResponseCode.FormatError, message) { }

    public InvalidFormatException(string message, Exception innerException)
        : base(HttpStatusCode.BadRequest, ResponseCode.FormatError, message, innerException) { }
}

public class InvalidArgumentException : AppException
{
    public InvalidArgumentException(string message)
        : base(HttpStatusCode.BadRequest, ResponseCode.ArgumentError, message) { }
}

public class ValidationFailedException : AppException
{
    public ValidationFailedException(string message)
        : base(HttpStatusCode.BadRequest, ResponseCode.ValidationError, message) { }
}
