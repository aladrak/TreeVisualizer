namespace TreeVisualizer.Domain.Visualization;

public sealed class TreeOperationStep(string description, TreeSnapshot snapshot)
{
    public string Description { get; } = description;

    public TreeSnapshot Snapshot { get; } = snapshot;
}
