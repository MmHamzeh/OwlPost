namespace OwlPost.Core.Interfaces;

public abstract class IFileDto : IDto<long>
{
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public long Size { get; set; }
    public MimeTypeEnum MimeType { get; set; }

    public byte[] FileContext { get; set; }
    public string Description { get; set; }

    protected override void PrepareDto(long currentUserId, TimeProvider timeProvider)
    {
        base.PrepareDto(currentUserId, timeProvider);
    }

}
