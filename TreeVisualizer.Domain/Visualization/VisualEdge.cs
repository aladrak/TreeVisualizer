namespace TreeVisualizer.Domain.Visualization;

public sealed class VisualEdge(string fromId, string toId)
{
    public string FromId { get; } = fromId;

    public string ToId { get; } = toId;
}
