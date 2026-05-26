using TreeVisualizer.Domain.Enums;

namespace TreeVisualizer.Domain.Visualization;

public sealed class VisualNode(
    string id,
    IReadOnlyList<int> keys,
    double x,
    double y,
    double width,
    double height,
    NodeVisualState state = NodeVisualState.Normal)
{
    public string Id { get; } = id;

    public IReadOnlyList<int> Keys { get; } = keys;

    public double X { get; } = x;

    public double Y { get; } = y;

    public double Width { get; } = width;

    public double Height { get; } = height;

    public NodeVisualState State { get; } = state;
}
