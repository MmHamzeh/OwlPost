namespace OwlPost.Core.Interfaces;

public abstract class IModelDto : IDto<long>
{

    public DateTime? CreatedOn { get; set; }
    public long? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public long? ModifiedBy { get; set; }

    protected override void PrepareDto(long currentUserId, TimeProvider timeProvider)
    {
        base.PrepareDto(currentUserId, timeProvider);


        if (PublicId is null || PublicId.Value == Guid.Empty) //create
        {
            CreatedBy = currentUserId;
            CreatedOn = timeProvider.GetUtcNow().DateTime;
        }
        else //update
        {
            ModifiedBy = currentUserId;
            ModifiedOn = timeProvider.GetUtcNow().DateTime;
        }

        return;
    }
}
