namespace TreeVisualizer.Domain.Visualization;

public sealed class TreeSnapshot(IReadOnlyList<VisualNode> nodes, IReadOnlyList<VisualEdge> edges)
{
    public IReadOnlyList<VisualNode> Nodes { get; } = nodes;

    public IReadOnlyList<VisualEdge> Edges { get; } = edges;

    public static TreeSnapshot Empty { get; } = new(Array.Empty<VisualNode>(), Array.Empty<VisualEdge>());
}
