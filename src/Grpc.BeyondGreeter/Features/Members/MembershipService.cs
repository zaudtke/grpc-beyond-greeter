using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.BeyondGreeter.Library;
using Grpc.BeyondGreeter.Members;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Grpc.BeyondGreeter.Features.Members;

public class MembershipService : Membership.MembershipBase
{
    private readonly ILogger<MembershipService> _logger;

    public MembershipService(ILogger<MembershipService> logger)
    {
        _logger = logger;
    }
    public override Task<CreateProfileResponse> CreateProfile(CreateProfileRequest request, ServerCallContext context)
    {
        var failures = new FailureCollection();
        // Business Rules that should be elsewhere
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            failures.Add(new Failure("Email", "Email is required."));
        }
            
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            failures.Add(new Failure("FirstName", "First Name is required."));
        }
            
        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            failures.Add(new Failure("LastName", "Last Name is required."));
        }

        if (!request.AcceptedTerms)
        {
            failures.Add(new Failure("AcceptedTerms", "Terms must be accepted."));
        }

        Result<CreateProfileResponse> result;
        if (failures.Any())
        {
            result = Result.Failure<CreateProfileResponse>(failures);
        }
        else
        {
            result = Result.Success(new CreateProfileResponse
            {
                Id = Guid.NewGuid().ToString()
            });
        }

        return Task.FromResult(
            result.Switch(
                ServiceEndpoint.Success,
                ServiceEndpoint.Fail<CreateProfileResponse>,
                ServiceEndpoint.Throw<CreateProfileResponse>
            ));
    }
}