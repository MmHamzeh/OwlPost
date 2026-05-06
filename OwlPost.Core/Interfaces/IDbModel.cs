namespace OwlPost.Core.Interfaces;

public interface IDbModel : IDbTable<long>
{
    public long CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }

}