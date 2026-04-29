namespace OwlPost.Core.Interfaces;

public interface IDbTable
{
    public Guid PublicId { get; set; }
}


public interface IDbTable<T> : IDbTable
{
    public T Id { get; set; }
}