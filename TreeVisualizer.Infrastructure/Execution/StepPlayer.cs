using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Infrastructure.Execution;

/// <summary>
/// Хранит последовательность шагов и выдает следующий шаг по запросу интерфейса.
/// </summary>
public sealed class StepPlayer
{
    private IReadOnlyList<TreeOperationStep> _steps = Array.Empty<TreeOperationStep>();
    private int _index = -1;

    public int CurrentIndex => _index;

    public int Count => _steps.Count;

    public bool HasSteps => _steps.Count > 0;

    public void Load(IReadOnlyList<TreeOperationStep> steps)
    {
        _steps = steps;
        _index = -1;
    }

    public TreeOperationStep? First()
    {
        if (_steps.Count == 0)
            return null;

        _index = 0;
        return _steps[_index];
    }

    public TreeOperationStep? Next()
    {
        if (_steps.Count == 0)
            return null;

        if (_index < _steps.Count - 1)
            _index++;

        return _steps[_index];
    }

    public void Reset()
    {
        _index = -1;
    }
}
