namespace NeonGadgetStore.Web.Services;

public class ToastService
{
    public event Action<ToastMessage>? OnShow;
    public event Action<Guid>? OnHide;

    public void ShowSuccess(string title, string? message = null)
    {
        Show(new ToastMessage
        {
            Id = Guid.NewGuid(),
            Title = title,
            Message = message,
            Type = ToastType.Success
        });
    }

    public void ShowError(string title, string? message = null)
    {
        Show(new ToastMessage
        {
            Id = Guid.NewGuid(),
            Title = title,
            Message = message,
            Type = ToastType.Error
        });
    }

    public void ShowInfo(string title, string? message = null)
    {
        Show(new ToastMessage
        {
            Id = Guid.NewGuid(),
            Title = title,
            Message = message,
            Type = ToastType.Info
        });
    }

    private void Show(ToastMessage toast)
    {
        OnShow?.Invoke(toast);
    }

    public void Hide(Guid id)
    {
        OnHide?.Invoke(id);
    }
}

public class ToastMessage
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public ToastType Type { get; set; }
}

public enum ToastType
{
    Success,
    Error,
    Info
}
