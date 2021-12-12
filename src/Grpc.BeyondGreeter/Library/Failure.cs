using System.Collections;
using System.Collections.Generic;

namespace Grpc.BeyondGreeter.Library;
// Paired down version of Classes/Records that live in an external library

public record Failure(string Field, string Message);
public record ConflictFailure(string Field, string Message) : Failure(Field, Message);
public record ForeignKeyFailure(string Field, string Message) : Failure(Field, Message);

public class FailureCollection : IEnumerable<Failure>
{
    private readonly List<Failure> _failures;

    public FailureCollection()
    {
        _failures = new List<Failure>();
    }

    public FailureCollection(IEnumerable<Failure> failures)
    {
        _failures = new List<Failure>(failures);
    }
        
    public void Add(Failure error)
    {
        _failures.Add(error);
    }
    public void AddRange(IEnumerable<Failure> range)
    {
        _failures.AddRange(range);
    }
    public IEnumerator<Failure> GetEnumerator()
    {
        return _failures.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}