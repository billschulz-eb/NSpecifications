using System.IO;

namespace NSpecifications;

public class Error : IError
{
    private readonly int _errorCode;
    private readonly string _errorMessage;
    
    public static readonly IError Null = new Error(0, "<null>");
    public static readonly IError None = new Error(0, "<none>");
    
    public Error(int errorCode, string errorMessage)
    {
        _errorCode = errorCode;
        _errorMessage = errorMessage;
    }

    public int ErrorCode => _errorCode;
    public string ErrorMessage => _errorMessage;
}