namespace OwlPost.Core.Models;

internal class ChatMessageHistory
{
    #region IDbModel properties
    
    public long CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public long Id { get; set; }
    public Guid PublicId { get; set; }

    #endregion


    #region Relations

    public long ChatMessageId { get; set; }
    public ChatMessage ChatMessage { get; set; }


    #endregion
}
