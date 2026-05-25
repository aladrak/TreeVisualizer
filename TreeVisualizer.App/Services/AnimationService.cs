namespace TreeVisualizer.App.Services;

public sealed class AnimationService
{
    public IDispatcherTimer CreateTimer(IDispatcher dispatcher, TimeSpan interval, Action tick)
    {
        IDispatcherTimer timer = dispatcher.CreateTimer();
        timer.Interval = interval;
        timer.Tick += (_, _) => tick();
        return timer;
    }
}
