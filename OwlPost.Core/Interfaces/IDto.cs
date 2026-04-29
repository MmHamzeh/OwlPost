using System.Text;

namespace OwlPost.Core.Interfaces;

public abstract class IDto<T>
{
    public Guid? PublicId { get; set; }
    private StringBuilder? ErrorMessages;


    public virtual bool IsValid()
        => ErrorMessages?.Length == 0;

    public virtual void PrepareDto(T currentUserId)
    {
        if (PublicId is null or PublicId.Value == Guid.Empty) //create
        {
            PublicId = Guid.CreateVersion7();
        }
        else //update
        {
            //ignored
        }

        return;
    }

    protected void AddErrorMessage(string errorMessages)
    {
        ErrorMessages ??= new StringBuilder();
        ErrorMessages.AppendLine(errorMessages);
    }

    public bool HasErrors()
        => ErrorMessages?.Length > 0;

    public string GetErrorMessage()
        => ErrorMessages?.ToString() ?? string.Empty;

}
