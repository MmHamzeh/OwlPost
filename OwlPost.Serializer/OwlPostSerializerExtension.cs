using Microsoft.Extensions.DependencyInjection;
using OwlPost.Core.ServicesContract;

namespace OwlPost.Serializer;

public static class OwlPostSerializerExtension
{

    public static void AddOwlPostSerializer(this IServiceCollection services)
    {
        services.AddLZ4Compressor();            //package : EasyCompressor.LZ4

        //services.AddSnappierCompressor();     //package : EasyCompressor.Snappier
        //services.AddZstdSharpCompressor();    //package : EasyCompressor.ZstdSharp
        //services.AddBrotliCompressor();       //package : EasyCompressor
        //services.AddGZipCompressor();         //package : EasyCompressor
        //services.AddDeflateCompressor();      //package : EasyCompressor
        //services.AddZLibCompressor();         //package : EasyCompressor
        //services.AddLZMACompressor();         //package : EasyCompressor.LZMA
        //services.AddZstdCompressor();         //package : EasyCompressor.Zstd (deprecated)
        //services.AddSnappyCompressor();       //package : EasyCompressor.Snappy (deprecated)
        //services.AddBrotliNETCompressor();    //package : EasyCompressor.BrotliNET (deprecated)

        services.AddSingleton<ISerializer, Serializer>();


    }
}
