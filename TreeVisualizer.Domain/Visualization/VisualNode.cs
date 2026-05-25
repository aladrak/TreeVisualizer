using TreeVisualizer.Domain.Enums;

namespace TreeVisualizer.Domain.Visualization;

/// <summary>
/// Узел, подготовленный для отрисовки на холсте.
/// </summary>
public sealed class VisualNode
{
    public VisualNode(
        string id,
        IReadOnlyList<int> keys,
        double x,
        double y,
        double width,
        double height,
        NodeVisualState state = NodeVisualState.Normal)
    {
        Id = id;
        Keys = keys;
        X = x;
        Y = y;
        Width = width;
        Height = height;
        State = state;
    }

    public string Id { get; }

    public IReadOnlyList<int> Keys { get; }

    public double X { get; }

    public double Y { get; }

    public double Width { get; }

    public double Height { get; }

    public NodeVisualState State { get; }
}
