using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using Grpc.BeyondGreeter.Library;
using Grpc.Core;

namespace Grpc.BeyondGreeter
{
    // these functions mostly throw exceptions.
    // in some cases they convert Failures to MetaData then Throw
    // They all have return types so that they can be used in 
    // .Switch statements in the Services using Method Group Syntax which is "cleaner"
    
    // Success just returns the value.  It is a wrapper so the Success Case on Switch can be a method group
    
    // Additional helpers added
    // THis is almost like a base class, but can't be due to Grpc Services being Generated
    
    
    public static class ServiceEndpoint
    {
        public static T Throw<T>(Exception exception)
        {
            ExceptionDispatchInfo.Throw(exception);
            throw exception;  // Make Compiler happy
        }
        
        public static T Fail<T>(FailureCollection failures)
        {
            var trailers = new Metadata();
            foreach (var failure in failures)
            {
                trailers.Add(failure.Field, failure.Message);
            }

            if (failures.Any(f => f is ConflictFailure))
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Conflict"), trailers);
            }

            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Arguments"), trailers);

        }
        
        public static T NotFound<T>()
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Item not found"));
        }

        public static T Success<T>(T value)
        {
            return value;
        }

        public static Guid ParseCompanyIdOrThrow(string input)
        {
            if (Guid.TryParse(input, out var companyId))
                return companyId;

            var trailers = new Metadata
            {
                {"CompanyId", $"Could not parse \"{input}\" as a CompanyId"}
            };
            throw new RpcException(new Status(StatusCode.InvalidArgument, "CompanyId Parse Error"), trailers);
        }
    }
    
    
}