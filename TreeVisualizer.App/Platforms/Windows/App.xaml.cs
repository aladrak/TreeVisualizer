using Microsoft.Maui;
using Microsoft.UI.Xaml;

namespace TreeVisualizer.App.WinUI;

/// <summary>
/// Windows-точка входа MAUI-приложения.
/// Основной интерфейс приложения создается на C# без XAML-страниц.
/// </summary>
public partial class App : MauiWinUIApplication
{
    public App()
    {
        InitializeComponent();
    }

    protected override MauiApp CreateMauiApp()
    {
        return TreeVisualizer.App.MauiProgram.CreateMauiApp();
    }
}
