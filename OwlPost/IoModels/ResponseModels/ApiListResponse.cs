namespace OwlPost.IoModels.ResponseModels;

public interface IApiListResponse<TModel> : IApiResponse where TModel : class, IVm, new()
{
    public List<TModel> ModelList { get; set; }
}

public record ApiListResponse<TModel> : IApiListResponse<TModel> where TModel : class, IVm, new()
{
    #region Ctor

    public ApiListResponse()
    {

    }

    public ApiListResponse(List<TModel> modelList, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        ModelList = modelList;
        StatusCode = statusCode;
        HasError = false;
        ExceptionMessage = string.Empty;
    }

    public ApiListResponse(Exception exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        ModelList = null;
        StatusCode = statusCode;
        HasError = true;
        ExceptionMessage = exception.Message;
    }

    #endregion

    public string? Message { get; set; }
    public bool HasError { get; set; }
    public string? ExceptionMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public List<TModel>? ModelList { get; set; }
}

