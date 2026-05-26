namespace TreeVisualizer.App.Services;

public sealed class AnimationService
{
    public static IDispatcherTimer CreateTimer(IDispatcher dispatcher, TimeSpan interval, Action tick)
    {
        var timer = dispatcher.CreateTimer();
        timer.Interval = interval;
        timer.Tick += (_, _) => tick();
        return timer;
    }
}
