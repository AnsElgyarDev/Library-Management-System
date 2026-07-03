namespace Library.Core.Common;

public class Result
{

    public bool isSuccess { get; set; }    
    public string? ErrorMessage { get; set; }    
    public int StatusCode { get; set; }

    public static Result Success(int statusCode = 200)
    {
        return new Result
        {
            StatusCode = statusCode,
            isSuccess = true
        };
    }    
    public static Result Failure(string errorMessage, int statusCode = 400)
    {
        return new Result
        {
            isSuccess = false,
            ErrorMessage = errorMessage,
            StatusCode = statusCode
        };
    } 
} 