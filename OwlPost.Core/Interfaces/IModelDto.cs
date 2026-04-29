namespace OwlPost.Core.Interfaces;

public abstract class IModelDto : IDto<long>
{

    public DateTime? CreatedOn { get; set; }
    public long? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public long? ModifiedBy { get; set; }

    public override void PrepareDto(long currentUserId)
    {
        base.PrepareDto(currentUserId);


        if (PublicId is null || PublicId.Value == Guid.Empty) //create
        {
            CreatedBy = currentUserId;
            CreatedOn = DateTime.Now;
        }
        else //update
        {
            ModifiedBy = currentUserId;
            ModifiedOn = DateTime.Now;
        }

        return;
    }
}
