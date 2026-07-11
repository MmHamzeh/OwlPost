namespace OwlPost.Core.ServicesContract;

public interface ISerializer
{
    /// <summary>
    /// The content type of the serialized data.
    /// This is typically used to indicate the media type of the data being serialized, such as "application/json" for JSON data or "application/octet-stream" for binary data.
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// The content encoding of the serialized data.
    /// This is typically used to indicate the encoding or compression method applied to the data, such as "gzip" for GZIP-compressed data or "deflate" for Deflate-compressed data.
    /// </summary>
    string ContentEncoding { get; }


    /// <summary>
    /// Serializes an object of type T to a byte array, compressing the serialized data using the specified compression method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="plainObject"></param>
    /// <returns></returns>
    byte[] Serialize<T>(T plainObject);

    /// <summary>
    /// Deserializes a byte array to an object of type T, decompressing the data using the specified compression method before deserialization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializedObject"></param>
    /// <returns></returns>
    T? Deserialize<T>(byte[] serializedObject);


    /// <summary>
    /// Serializes an object of type T to a Stream, compressing the serialized data using the specified compression method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="plainObject"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<Stream> SerializeAsync<T>(T plainObject, CancellationToken ct = default);

    /// <summary>
    /// Deserializes a Stream to an object of type T, decompressing the data using the specified compression method before deserialization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="streamedObject"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<T?> DeserializeAsync<T>(Stream streamedObject, CancellationToken ct = default);
}