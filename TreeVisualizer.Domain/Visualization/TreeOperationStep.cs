namespace TreeVisualizer.Domain.Visualization;

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
