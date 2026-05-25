namespace TreeVisualizer.Domain.Visualization;

/// <summary>
/// Снимок дерева в конкретный момент выполнения операции.
/// </summary>
public sealed class TreeSnapshot
{
    public TreeSnapshot(IReadOnlyList<VisualNode> nodes, IReadOnlyList<VisualEdge> edges)
    {
        Nodes = nodes;
        Edges = edges;
    }

    public IReadOnlyList<VisualNode> Nodes { get; }

    public IReadOnlyList<VisualEdge> Edges { get; }

    public static TreeSnapshot Empty { get; } = new(Array.Empty<VisualNode>(), Array.Empty<VisualEdge>());
}
