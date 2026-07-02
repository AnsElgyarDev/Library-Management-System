namespace Practice.Common;
public class Result<T> : Result
{
    public T? Data { get; set; }
    public static Result<T> GenericSuccess(T? data, int statusCode = 200) =>
    new Result<T>
    {
        Data = data,
        isSuccess = true, 
        StatusCode = statusCode
    };

    public static Result<T> GenericFailure(string errorMessage, int statusCode = 400) =>
    new Result<T>
    {
        isSuccess = false, 
        ErrorMessage = errorMessage,
        StatusCode = statusCode
    };         
}