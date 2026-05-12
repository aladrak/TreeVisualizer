using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;

namespace TreeVisualizer.Domain.Visualization;

public record VisualNode(
    int Id,
    ITreeNode Node,
    double X,
    double Y,
    double Width = 40,
    double Height = 40,
    NodeState State = NodeState.Default);