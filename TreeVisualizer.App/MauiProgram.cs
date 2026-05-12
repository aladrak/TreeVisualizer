using Microsoft.Extensions.Logging;
using TreeVisualizer.App.ViewModels;
using TreeVisualizer.App.Views;
using TreeVisualizer.Infrastructure.Factories;
using TreeVisualizer.Infrastructure.Layout;

namespace TreeVisualizer.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<ITreeLayoutService, BinaryTreeLayoutService>();
        builder.Services.AddSingleton<TreeFactory>();
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}