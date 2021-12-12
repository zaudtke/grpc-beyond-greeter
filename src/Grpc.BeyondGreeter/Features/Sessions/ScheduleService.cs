using System;
using System.Threading.Tasks;
using Grpc.BeyondGreeter.Library;
using Grpc.BeyondGreeter.Sessions;
using Grpc.Core;

namespace Grpc.BeyondGreeter.Features.Sessions;

public class ScheduleService : BeyondGreeter.Sessions.ScheduleService.ScheduleServiceBase
{
    public override async Task<ScheduleSessionResponse> ScheduleSession(ScheduleSessionRequest request, ServerCallContext context)
    {
        var dbSession = await GetSession(request.SessionId);
        dbSession.Date = request.Date.ToDateTime();
        dbSession.Duration = request.Duration.ToTimeSpan();
        dbSession.Room = $"Room {Guid.NewGuid()}";
        await Save(dbSession);
        var result = Result.Success(new ScheduleSessionResponse
        {
            SessionId = request.SessionId,
            Date = request.Date,
            Duration = request.Duration,
            RoomName = dbSession.Room
        });
        return
            result.Switch(
                ServiceEndpoint.Success,
                ServiceEndpoint.Fail<ScheduleSessionResponse>,
                ServiceEndpoint.Throw<ScheduleSessionResponse>
            );
    }



    private Task<Session> GetSession(string id)
    {
        return Task.FromResult(new Session {Id = Guid.NewGuid().ToString()});
    }

    private Task Save(Session session)
    {
        return Task.CompletedTask;
    }
}


public class Session
{
    public Session()
    {
        Id = Guid.NewGuid().ToString();
        Room = string.Empty;
    }
    public string Id { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan Duration { get; set; }
    public string Room { get; set; }
}