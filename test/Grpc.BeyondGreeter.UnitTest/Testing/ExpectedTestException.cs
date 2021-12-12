using System;

namespace Grpc.BeyondGreeter.UnitTest.Testing;

public class ExpectedTestException : Exception
{
    public const string ExpectedMessage = "This is an expected exception";

    public ExpectedTestException() : base(ExpectedMessage)
    {
    }
}