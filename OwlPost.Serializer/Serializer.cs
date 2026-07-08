using EasyCompressor;
using System.Text.Json;
using OwlPost.Core.ServicesContract;

namespace OwlPost.Serializer;

public class Serializer : ISerializer
{
    private readonly ICompressor _compressor = new LZ4Compressor();

    public byte[] Serialize<T>(T plainObject)
    {
        var bytes = ObjectToByteArray(plainObject);
        var compressedBytes = _compressor.Compress(bytes);
        return compressedBytes;

    }

    public T? Deserialize<T>(byte[] serialized)
    {
        var uncompressedBytes = _compressor.Decompress(serialized);
        var plainObject = ByteArrayToObject<T>(uncompressedBytes);
        return plainObject;
    }

    public async Task<Stream> SerializeAsync<T>(T plainObject)
    {
        await using var inputStream = new MemoryStream();
        await ObjectToStreamAsync(plainObject, inputStream);
        var outputStream = new MemoryStream();
        await _compressor.CompressAsync(inputStream, outputStream);
        return outputStream;

    }

    public async Task<T?> DeserializeAsync<T>(Stream inputStream)
    {
        var outputStream = new MemoryStream();
        await _compressor.DecompressAsync(inputStream, outputStream);
        var plainObject = await StreamToObjectAsync<T>(outputStream);
        return plainObject;
    }


    #region Private Methods

    private static byte[] ObjectToByteArray<T>(T obj)
    {
        return obj == null
            ? []
            : JsonSerializer.SerializeToUtf8Bytes(obj);
    }

    private static T? ByteArrayToObject<T>(byte[] bytes)
    {
        return bytes.Length == 0
            ? default
            : JsonSerializer.Deserialize<T>(bytes);
    }



    private async Task ObjectToStreamAsync<T>(T obj, Stream stream)
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        await JsonSerializer.SerializeAsync(stream, obj);
        //await stream.FlushAsync();
    }

    private async Task<T?> StreamToObjectAsync<T>(Stream stream)
    {
        if (stream is null)
            throw new ArgumentNullException(nameof(stream));

        return await JsonSerializer.DeserializeAsync<T>(stream);
    }

    #endregion

}
