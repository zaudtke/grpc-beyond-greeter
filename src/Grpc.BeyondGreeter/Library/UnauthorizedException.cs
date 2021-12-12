using System;

namespace Grpc.BeyondGreeter.Library;

// Paired down version of Class that lives in an external library

public class UnauthorizedException : Exception
{
    public UnauthorizedException()
    {
    }

    public UnauthorizedException(Exception innerException)
        : base(string.Empty, innerException)
    {
    }
}