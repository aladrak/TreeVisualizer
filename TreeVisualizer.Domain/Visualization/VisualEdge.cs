namespace TreeVisualizer.Domain.Visualization;

/// <summary>
/// Ребро между двумя визуальными узлами.
/// </summary>
public sealed class VisualEdge
{
    public VisualEdge(string fromId, string toId)
    {
        FromId = fromId;
        ToId = toId;
    }

    public string FromId { get; }

    public string ToId { get; }
}
