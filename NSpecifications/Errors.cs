using System.Collections;

namespace NSpecifications;

public class Errors : IError, IEnumerable<Error>
{
    public Errors(params IError[] errors)
    {
        _errors = Array.Empty<Error>();
        _errors = errors.SelectMany(x => x switch
                                         {
                                             Error err => _errors.Append(err),
                                             Errors errs => _errors.Concat(errs)
                                         }).ToArray();
    }

    private readonly Error[] _errors;

    public IEnumerator<Error> GetEnumerator()
    {
        foreach(var error in _errors)
        {
            yield return error;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public string ErrorMessage => _errors.Select(e => e.ErrorMessage).Aggregate((l, r) => l + ", " + r);
}