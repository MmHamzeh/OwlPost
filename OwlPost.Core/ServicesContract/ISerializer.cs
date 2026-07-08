namespace OwlPost.Core.ServicesContract;

public interface ISerializer
{
    byte[] Serialize<T>(T plainObject);
    T? Deserialize<T>(byte[] serialized);
    Task<Stream> SerializeAsync<T>(T plainObject);
    Task<T?> DeserializeAsync<T>(Stream inputStream);
}