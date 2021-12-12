using System.Threading.Tasks;
using Grpc.BeyondGreeter.Greetings;
using Grpc.BeyondGreeter.Library;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Grpc.BeyondGreeter.Features.Greetings;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
    {
        var reply = new HelloResponse
        {
            Message = $"Hello {request.Name}"
        };

        var result = Result.Success(reply);  // Only needed to show Switch  Normally Handler returns Result
        return Task.FromResult(
            result.Switch(
                ServiceEndpoint.Success,
                ServiceEndpoint.Fail<HelloResponse>,
                ServiceEndpoint.Throw<HelloResponse>)
        );
    }

    public override Task<HelloDefaultsResponse> SayHelloDefaults(HelloDefaultsRequest request, ServerCallContext context)
    {
        var reply = new HelloDefaultsResponse
        {
            AcceptedTerms = request.AcceptedTerms,
            Age = request.Age,
            Name = request.Name
        };
            
        var result = Result.Success(reply);  // Only needed to show Switch  Normally Handler returns Result
        return Task.FromResult(
            result.Switch(
                ServiceEndpoint.Success,
                ServiceEndpoint.Fail<HelloDefaultsResponse>,
                ServiceEndpoint.Throw<HelloDefaultsResponse>)
        );
    }

    public override Task<HelloNullablesResponse> SayHelloNullables(HelloNullablesRequest request, ServerCallContext context)
    {
        var reply = new HelloNullablesResponse()
        {
            Age = request.Age,
            AcceptedTerms = request.AcceptedTerms,
            Name = request.Name
        };

        var result = Result.Success(reply);
        return Task.FromResult(
            result.Switch(
                ServiceEndpoint.Success,
                ServiceEndpoint.Fail<HelloNullablesResponse>,
                ServiceEndpoint.Throw<HelloNullablesResponse>)
        );
    }

    public override Task<HelloOptionalsResponse> SayHelloOptionals(HelloOptionalsRequest request, ServerCallContext context)
    {
        var reply = new HelloOptionalsResponse()
        {
            Age = request.Age,
            AcceptedTerms = request.AcceptedTerms,
            Name = request.Name
        };

        var result = Result.Success(reply);
        return Task.FromResult(
            result.Switch(
                ServiceEndpoint.Success,
                ServiceEndpoint.Fail<HelloOptionalsResponse>,
                ServiceEndpoint.Throw<HelloOptionalsResponse>)
        );
    }

    public override Task<HelloNestedResponse> SayHelloNested(HelloNestedRequest request, ServerCallContext context)
    {
        var reply = new HelloNestedResponse
        {
            Id = request.Id,
            Name = request.Name
        };
        var result = Result.Success(reply);
        return Task.FromResult(
            result.Switch(
                ServiceEndpoint.Success,
                ServiceEndpoint.Fail<HelloNestedResponse>,
                ServiceEndpoint.Throw<HelloNestedResponse>)
        );
    }
}