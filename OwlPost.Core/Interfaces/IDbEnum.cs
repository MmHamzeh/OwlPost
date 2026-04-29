namespace OwlPost.Core.Interfaces;

public interface IDbEnum<T> : IDbTable<T> where T : Enum
{
    public string Title { get; set; }
    public string TitleEn { get; set; }
    public string Description { get; set; }
}