namespace OwlPost.Core.Models;

public class ChatMessage : IDbModel
{
    #region Ctor

    public ChatMessage()
    {
        PublicId = Guid.CreateVersion7();

        Replies = [];
        ChatMessageHistories = [];
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

    public bool IsDeleted { get; set; }
    public string Content { get; set; }

    /// <summary>
    /// ConcurrencyToken
    /// </summary>
    public Guid Version { get; set; }

    #region Relations

    public User User { get; set; }

    // parent message (reply)
    public long? ParentMessageId { get; set; }
    public ChatMessage? ParentMessage { get; set; }

    // replies
    public ICollection<ChatMessage> Replies { get; set; } = [];

    public long ChatRoomId { get; set; }
    public ChatRoom ChatRoom { get; set; }

    public ICollection<ChatMessageHistory> ChatMessageHistories { get; set; } = [];

    #endregion

}
