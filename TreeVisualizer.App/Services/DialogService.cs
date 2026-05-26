namespace TreeVisualizer.App.Services;

public sealed class DialogService
{
    public static Task ShowErrorAsync(Page page, string message)
    {
        return page.DisplayAlertAsync("Ошибка", message, "ОК");
    }
}