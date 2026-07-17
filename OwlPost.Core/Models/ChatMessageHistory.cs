namespace OwlPost.Core.Models;

public class ChatMessageHistory
{
    #region Ctor

    public ChatMessageHistory()
    {
        PublicId = Guid.CreateVersion7();
    }

    public ChatMessageHistory(ChatMessage message, long currentUserId, TimeProvider timeProvider) 
        : this()
    {
        ChatMessage = message;
        ChatMessageId = message.Id;
        OldContent = message.Content;
        CreatedOn = timeProvider.GetUtcNow().DateTime;
        CreatedBy = currentUserId;
    }

    #endregion

    #region IDbModel properties

    public long CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public long Id { get; set; }
    public Guid PublicId { get; set; }

    #endregion

    public string OldContent { get; set; }

    #region Relations

    public long ChatMessageId { get; set; }
    public ChatMessage ChatMessage { get; set; }


    #endregion
}
