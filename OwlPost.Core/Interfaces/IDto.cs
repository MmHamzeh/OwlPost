using System.Text;

namespace OwlPost.Core.Interfaces;

public abstract class IDto<T>
{
    public virtual Guid? PublicId { get; set; }
    private StringBuilder? _errorMessages;


    protected bool IsValid()
        => _errorMessages?.Length == 0;

    protected virtual void PrepareDto(T currentUserId, TimeProvider timeProvider)
    {
        if (PublicId is null || PublicId == Guid.Empty) //create
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
        _errorMessages ??= new StringBuilder();
        _errorMessages.AppendLine(errorMessages);
    }

    protected bool HasErrors()
        => _errorMessages?.Length > 0;

    protected string GetErrorMessage()
        => _errorMessages?.ToString() ?? string.Empty;

}
