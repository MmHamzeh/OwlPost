namespace OwlPost.Core.Interfaces;

public interface IDbFile : IDbTable<long>
{
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public long Size { get; set; }
    public MimeTypeEnum MimeType { get; set; }

    public byte[] FileContext { get; set; }
    public string Description { get; set; }
}
