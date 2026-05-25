namespace TreeVisualizer.App.Services;

public sealed class DialogService
{
    public Task ShowErrorAsync(Page page, string message)
    {
        return page.DisplayAlert("Ошибка", message, "ОК");
    }
}
