namespace OwlPost.Core.Interfaces;

public interface IVm
{

}

public interface IViewModel : IVm
{

    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public Guid? ModifiedBy { get; set; }
}

public interface IEnmVm<T> : IVm where T : Enum
{
    public string Title { get; set; }
    public string TitleEn { get; set; }
    public string Description { get; set; }
}

public interface IFileVm : IVm
{
    public string FileName { get; set; }
    public string FileExtension { get; set; }
    public long Size { get; set; }
    public MimeTypeEnum MimeType { get; set; }

    public byte[] FileContext { get; set; }
    public string Description { get; set; }
}
