using System;

namespace Grpc.BeyondGreeter.Library
{
    // Paired down version of Classes/Records that live in an external library
    
    public enum ResultStatus
    {
        Success,
        Failure,
        Exception
    }

    public class Result
    {
	    private readonly FailureCollection? _failures;
	    private readonly Exception? _exception;

	    public ResultStatus Status { get; }

	    public FailureCollection Failures
	    {
		    get
		    {
			    if (Status == ResultStatus.Failure && _failures is not null)
			    {
				    return _failures;
			    }

			    throw new InvalidOperationException("Failures should not be called if status isn't Failure");
		    }
	    }

	    public Exception Exception
	    {
		    get
		    {
			    if (Status == ResultStatus.Exception && _exception is not null)
			    {
				    return _exception;
			    }

			    throw new InvalidOperationException("Exception should be called if status isn't Exception");
		    }
	    }

	    protected Result(ResultStatus status, FailureCollection? failures, Exception? exception)
	    {
		    Status = status;
		    _failures = failures;
		    _exception = exception;
	    }

	    public static Result<T> Failure<T>(FailureCollection failures)
	    {
		    if (failures == null)
		    {
			    throw new ArgumentNullException(nameof(failures));
		    }

		    return new Result<T>(default, ResultStatus.Failure, failures, null);
	    }

	    public static Result<T> Success<T>(T? value)
	    {
		    if (value == null)
		    {
			    throw new ArgumentNullException(nameof(value));
		    }

		    return new Result<T>(value, ResultStatus.Success, null, null);
	    }

	    public static Result<T> Error<T>(Exception exception)
	    {
		    if (exception == null)
		    {
			    throw new ArgumentNullException(nameof(exception));
		    }

		    return new Result<T>(default, ResultStatus.Exception, null, exception);
	    }
    }
    
    public class Result<T> : Result
    {
	    private readonly T? _value;

	    protected internal Result(T? value, ResultStatus status, FailureCollection? failures, Exception? exception)
		    : base(status, failures, exception)
	    {
		    if (status == ResultStatus.Success && value == null)
			    throw new ArgumentNullException(nameof(value));

		    _value = value;
	    }

	    public T Value
	    {
		    get
		    {
			    if (Status == ResultStatus.Success  && _value is not null)
			    {
				    return _value;
			    }

			    throw new InvalidOperationException("Value should not be called if status isn't Success");
		    }
	    }
    }
    
    public static class ResultExtensions
	{
		public static U Switch<T, U>(this Result<T> @this, Func<T, U> onSuccess, Func<FailureCollection, U> onFailure, Func<Exception, U> onException)
		{
			if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
			if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));
			if (onException == null) throw new ArgumentNullException(nameof(onSuccess));

			switch (@this.Status)
			{
				case ResultStatus.Success:
					return onSuccess(@this.Value);
				case ResultStatus.Failure:
					return onFailure(@this.Failures);
				case ResultStatus.Exception:
					return onException(@this.Exception);
				default:
					throw new InvalidOperationException("Invalid Result Status");

			}
		}
	}
}
    