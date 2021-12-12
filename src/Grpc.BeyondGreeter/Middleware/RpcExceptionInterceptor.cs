using System;
using System.Threading.Tasks;
using Grpc.BeyondGreeter.Library;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Grpc.BeyondGreeter.Middleware;

public class RpcExceptionInterceptor : Interceptor
{
    private readonly ILogger<RpcExceptionInterceptor> _logger;

    public RpcExceptionInterceptor(ILogger<RpcExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        // Things going on here.
        // this middleware is in place around GRPC Calls only, and only Unary ones which is all we are using
        // Wraps the call in try catch block
        // Catch the exception and convert it to a RpcException.  This is the only type of exception that can be 
        // Returned in Grpc, and it would be wrapped in a RpcException with Status Undefined anyway.
        // We are handling the "known" exceptions and returning the correct the GrpStatus Code.  These are app defined
        // exceptions, and we really don't care about the Stack Trace on the Server, and don't need to be specific for the client
        // For the last case in an Exception we have no idea what happened we will ExceptionDispatch Throw without converting it.
        // This *should* allow the server to log everything correctly with the stack trace.
        // For the client, Unknown vs Internal Server Error, doesnt' really matter.  It's a server problem that 
        // Can't be fixed on the client, unlike a Permission Error, ETag Error, or Sql Conflict.
        // We are changing the underlying HttpStatus Code to the appropriate value.  This is to accomodate Application Insights.
        // If this isn't done, everything will look like a 200 OK to it.
            
        // RpcExceptions are thrown from Endpoint.Fail<T>
            
        // When clauses on the catch statements will not unwrap the stacktrace
        // Based on https://the-worst.dev/how-to-handle-grpc-errors-in-net-core/

        var httpContext = context.GetHttpContext();
        try
        {
            return await continuation(request, context);
        }
        catch (UnauthorizedException) when (
            httpContext.User.Identity?.IsAuthenticated == true)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                "User does not have permission to perform this action."));
        }
        catch (UnauthorizedException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not Authenticated"));
        }
        catch (SqlException sqlException) when (sqlException.Number == 547)
        {
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Cannot delete record"),
                BuildSqlExceptionMetaData(sqlException));
        }
        catch (RpcException rpcException) when (rpcException.StatusCode == StatusCode.FailedPrecondition)
        {
            // ETag-Failure
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            throw;
        }
        catch (RpcException rpcException) when (rpcException.StatusCode == StatusCode.InvalidArgument)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            throw;
        }
        catch (RpcException rpcException) when (rpcException.StatusCode == StatusCode.NotFound)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            throw;
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, e, e.Message);
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }
    }
        
    private static Metadata BuildSqlExceptionMetaData(SqlException exception)
    {
        // SQL Server Error Messages - Msg 547:
        //		The DELETE statement conflicted with the REFERENCE constraint "FK_TableName_ColumnName".The 
        //      conflict occurred in database "DB", table "dbo.Table", column 'ColumnName'. The 
        //      statement has been terminated.
            
        var message = exception.Message;

        // isolate the table and column to make a friendly message
        var tableIndex = message.IndexOf(", table", StringComparison.InvariantCultureIgnoreCase);
        if (tableIndex > 0)
        {
            var endOfColumnIndex = message.IndexOf("'.", tableIndex, StringComparison.Ordinal);
            if (endOfColumnIndex > 0)
            {
                var tableAndColumn = message.Substring(tableIndex + 2/*, */, endOfColumnIndex - tableIndex);

                // remove schema
                tableAndColumn = tableAndColumn.Replace("dbo.", string.Empty, StringComparison.InvariantCultureIgnoreCase);

                // remove trailing carriage return and new-line
                tableAndColumn = tableAndColumn.TrimEnd('\r', '\n');

                message = $"Record cannot be deleted due to referencing records in {tableAndColumn}";
            }
        }

        return new Metadata {{"", message}};
    }
}