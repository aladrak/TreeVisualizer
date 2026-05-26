using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.App.ViewModels;

public sealed class TreeOperationViewModel
{
    public IReadOnlyList<TreeOperationStep> Steps { get; set; } = Array.Empty<TreeOperationStep>();

    public int CurrentStepIndex { get; set; } = -1;
}
