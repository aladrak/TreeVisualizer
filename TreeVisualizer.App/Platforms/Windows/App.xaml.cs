using Microsoft.Maui;
using Microsoft.UI.Xaml;

namespace TreeVisualizer.App.WinUI;

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
