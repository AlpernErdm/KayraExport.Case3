namespace Shared.Models;

public class AuthResult<T>
{
    public bool IsSuccess { get; private set; }
    
    public string? ErrorMessage { get; private set; }
    
    public T? Data { get; private set; }
    
    public static AuthResult<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };
    
    public static AuthResult<T> Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}

public class AuthResult
{
    public bool IsSuccess { get; private set; }
    
    public string? ErrorMessage { get; private set; }
    
    public string? SuccessMessage { get; private set; }
    
    public static AuthResult Success(string successMessage) => new()
    {
        IsSuccess = true,
        SuccessMessage = successMessage
    };
    
    public static AuthResult Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}
