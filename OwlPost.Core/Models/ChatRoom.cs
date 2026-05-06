namespace OwlPost.Core.Models;

public class ChatRoom : IDbModel
{
        #region IDbModel properties
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long Id { get; set; }
        public Guid PublicId { get; set; }
        #endregion

        public bool IsArchived { get; set; }


        #region Relations

        public ICollection<ChatMessage> ChatMessages { get; set; }


        #endregion

}
