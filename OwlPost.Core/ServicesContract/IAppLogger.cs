using Microsoft.Extensions.Logging;

namespace OwlPost.Core.ServicesContract;

public interface IAppLogger<out T> : ILogger<T>
{

}
