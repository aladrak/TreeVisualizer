using TreeVisualizer.App.Views;

namespace TreeVisualizer.App;

public sealed class App : Application
{
    public App()
    {
        UserAppTheme = AppTheme.Light;
        MainPage = new MainPage();
    }
}
