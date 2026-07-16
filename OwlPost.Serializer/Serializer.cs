using EasyCompressor;
using System.Text.Json;
using System.Text.Json.Serialization;
using K4os.Compression.LZ4;
using Microsoft.IO;
using OwlPost.Core.ServicesContract;

namespace OwlPost.Serializer;

public class Serializer : ISerializer
{
    #region Ctor and Fields

    private readonly ICompressor _compressor;
    private readonly RecyclableMemoryStreamManager _streamManager;
    
    public Serializer(RecyclableMemoryStreamManager streamManager)
    {
        const LZ4Level lZ4Level = LZ4Level.L03_HC;
        const LZ4BinaryCompressionMode lZ4BinaryCompressionMode = LZ4BinaryCompressionMode.Optimal;

        _compressor = new LZ4Compressor(lZ4Level, lZ4BinaryCompressionMode);
        _streamManager = streamManager ?? throw new ArgumentNullException(nameof(streamManager));

        ContentType = "application/octet-stream";
        ContentEncoding = $"lz4-{lZ4BinaryCompressionMode.ToString().ToLowerInvariant()}";
    }

    #endregion

    public string ContentType { get; }
    public string ContentEncoding { get; }


    public byte[] Serialize<T>(T plainObject)
    {
        if (plainObject is null)
            return [];

        using var tempStream = _streamManager.GetStream("Serializer_Sync");
        JsonSerializer.Serialize(tempStream, plainObject, SerializerOptions.JsonSerializerOptions);
        tempStream.Position = 0;
        return _compressor.Compress(tempStream);
    }

    public T? Deserialize<T>(byte[]? serializedObject)
    {
        if (serializedObject is null || serializedObject.Length == 0)
            return default;

        using var compressedStream = new MemoryStream(serializedObject, writable: false);
        using var decompressedStream = _streamManager.GetStream("Serializer_Sync_Decompressed");

        _compressor.Decompress(compressedStream, decompressedStream);
        decompressedStream.Position = 0;

        return JsonSerializer.Deserialize<T>(decompressedStream, SerializerOptions.JsonSerializerOptions);
    }

    public async Task<Stream> SerializeAsync<T>(T plainObject, CancellationToken ct = default)
    {
        if (plainObject is null)
            return Stream.Null;

        await using var jsonTempStream = _streamManager.GetStream("Serializer_JsonTemp");

        await JsonSerializer.SerializeAsync(
            jsonTempStream,
            plainObject,
            SerializerOptions.JsonSerializerOptions,
            ct).ConfigureAwait(false);

        jsonTempStream.Position = 0;

        var compressedOutputStream = _streamManager.GetStream("Serializer_CompressedOutput");

        try
        {
            await _compressor
                .CompressAsync(jsonTempStream, compressedOutputStream, ct)
                .ConfigureAwait(false);

            compressedOutputStream.Position = 0;
            return compressedOutputStream;
        }
        catch
        {
            await compressedOutputStream.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    public async Task<T?> DeserializeAsync<T>(Stream streamedObject, CancellationToken ct = default)
    {
        if (streamedObject is null)
            throw new ArgumentNullException(nameof(streamedObject));

        if (streamedObject.CanSeek && streamedObject.Position >= streamedObject.Length)
            return default;

        await using var decompressedStream = _streamManager.GetStream("Serializer_Decompressed");

        await _compressor
            .DecompressAsync(streamedObject, decompressedStream, ct)
            .ConfigureAwait(false);

        if (decompressedStream.Length == 0)
            return default;

        decompressedStream.Position = 0;

        return await JsonSerializer.DeserializeAsync<T>(
            decompressedStream,
            SerializerOptions.JsonSerializerOptions,
            ct).ConfigureAwait(false);
    }

}
