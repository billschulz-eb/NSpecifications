using System.Collections;
using System.IO;

namespace NSpecifications;

public class Error
{
    private readonly int _errorCode;
    private readonly string _errorMessage;
    public static readonly Error Null = new(0, String.Empty);
    
    public Error(int errorCode, string errorMessage)
    {
        _errorCode = errorCode;
        _errorMessage = errorMessage;
    }

    public int ErrorCode => _errorCode;
    public string ErrorMessage => _errorMessage;
}

public class Errors : IEnumerable<Error>
{
    public Errors(params Error[] errors)
    {
        _errors = errors;
    }

    public Errors(Errors left, Error right)
    {
        _errors = left.Append(right)
                      .ToArray();
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
}