namespace OwlPost.IoModels.ResponseModels;

public interface IApiSingleResponse<TModel> : IApiResponse where TModel : class, IVm, new()
{
    public TModel Model { get; set; }
}

public record ApiSingleResponse<TModel> : IApiSingleResponse<TModel> where TModel : class, IVm, new()
{

    #region Ctor

    public ApiSingleResponse()
    {

    }

    public ApiSingleResponse(TModel model, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        Model = model;
        StatusCode = statusCode;
        HasError = false;
        ExceptionMessage = string.Empty;
    }

    public ApiSingleResponse(Exception exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        Model = null;
        StatusCode = statusCode;
        HasError = true;
        ExceptionMessage = exception.Message;
    }

    public ApiSingleResponse(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        Model = null;
        StatusCode = statusCode;
        HasError = true;
        Message = errorMessage;
    }

    #endregion

    public string? Message { get; set; }
    public bool HasError { get; set; }
    public string? ExceptionMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public TModel? Model { get; set; }
}

