namespace Shared.Models;

public class LogResult<T>
{
    public bool IsSuccess { get; private set; }
    
    public string? ErrorMessage { get; private set; }
    
    public T? Data { get; private set; }
    
    public static LogResult<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };
    
    public static LogResult<T> Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}

public class LogResult
{
    public bool IsSuccess { get; private set; }
    
    public string? ErrorMessage { get; private set; }
    
    public Guid? LogId { get; private set; }
    
    public static LogResult Success(Guid logId) => new()
    {
        IsSuccess = true,
        LogId = logId
    };
    
    public static LogResult Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}
