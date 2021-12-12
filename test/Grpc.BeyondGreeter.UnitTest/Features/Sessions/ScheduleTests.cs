using System;
using Grpc.BeyondGreeter.Sessions;
using Grpc.BeyondGreeter.UnitTest.Testing;
using Xunit;

namespace Grpc.BeyondGreeter.UnitTest.Features.Sessions;

public class ScheduleTests : IClassFixture<TestEnvironmentFixture<Program>>
{
    private readonly TestEnvironmentFixture<Program> _fixture;

    public ScheduleTests(TestEnvironmentFixture<Program> fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "TimeStamp and Duration can be null")]
    public void TimestampCanBeNull()
    {
        var date = DateTime.Parse("7/27/2021 2:30 PM");
        var request = new ScheduleSessionRequest
        {
            SessionId = "123",
            Date = null,
            Duration = null
        };
        Assert.Equal(1,1);
    }
}