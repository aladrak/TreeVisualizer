using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Infrastructure.Execution;

// Хранит последовательность шагов и выдает следующий шаг по запросу интерфейса.
public sealed class StepPlayer
{
    private IReadOnlyList<TreeOperationStep> _steps = Array.Empty<TreeOperationStep>();

    public int CurrentIndex { get; private set; } = -1;

    public int Count => _steps.Count;

    public bool HasSteps => _steps.Count > 0;

    public void Load(IReadOnlyList<TreeOperationStep> steps)
    {
        _steps = steps;
        CurrentIndex = -1;
    }

    public TreeOperationStep? First()
    {
        if (_steps.Count == 0)
            return null;

        CurrentIndex = 0;
        return _steps[CurrentIndex];
    }

    public TreeOperationStep? Next()
    {
        if (_steps.Count == 0)
            return null;

        if (CurrentIndex < _steps.Count - 1)
            CurrentIndex++;

        return _steps[CurrentIndex];
    }

    public void Reset()
    {
        CurrentIndex = -1;
    }
}
