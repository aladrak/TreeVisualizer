using Microsoft.Extensions.DependencyInjection;
using TreeVisualizer.App.Views;

namespace TreeVisualizer.App;

public partial class App : Application
{
    private readonly MainPage _mainPage;
    public App(MainPage mainPage) => _mainPage = mainPage;

    protected override Window CreateWindow(IActivationState? activationState) => new(_mainPage);
}