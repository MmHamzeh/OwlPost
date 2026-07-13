namespace OwlPost.IoModels.ResponseModels;

public interface IApiResponse
{
    public string? Message { get; set; }
    public bool HasError { get; set; }
    public string? ExceptionMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}



public record ApiResponse : IApiResponse
{
    public ApiResponse()
    {
        StatusCode = HttpStatusCode.OK;
    }

    public ApiResponse(Exception exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        StatusCode = statusCode;
        HasError = true;
        ExceptionMessage = exception.Message;
    }

    public ApiResponse(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        StatusCode = statusCode;
        HasError = true;
        ExceptionMessage = errorMessage;
    }


    public string? Message { get; set; }
    public ApiResponse(bool hasError, HttpStatusCode statusCode)
    {
        this.HasError = hasError;
        this.StatusCode = statusCode;

    }
    public bool HasError { get; set; }
    public string? ExceptionMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}

