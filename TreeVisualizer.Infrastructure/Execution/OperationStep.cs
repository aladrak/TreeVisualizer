using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Infrastructure.Execution;

public record OperationStep(
    IReadOnlyList<VisualNode> Snapshot,
    string Description,
    int? FocusedNodeId = null,
    NodeState? FocusedState = null);