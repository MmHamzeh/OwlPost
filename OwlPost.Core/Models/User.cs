namespace OwlPost.Core.Models;

public class User : IDbModel
{
    #region IDbModel properties

    public long CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public long Id { get; set; }
    public Guid PublicId { get; set; }

    #endregion


    public ICollection<ChatRoomUser> ChatRoomUsers { get; set; }

}