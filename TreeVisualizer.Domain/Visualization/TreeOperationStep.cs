namespace TreeVisualizer.Domain.Visualization;

/// <summary>
/// Один шаг выполнения операции над деревом.
/// </summary>
public sealed class TreeOperationStep
{
    public TreeOperationStep(string description, TreeSnapshot snapshot)
    {
        Description = description;
        Snapshot = snapshot;
    }

    public string Description { get; }

    public TreeSnapshot Snapshot { get; }
}
