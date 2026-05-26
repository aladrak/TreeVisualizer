using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Visualization;
using TreeVisualizer.Infrastructure.Factories;

namespace TreeVisualizer.Infrastructure.Execution;

public sealed class OperationExecutor
{
    public static IReadOnlyList<TreeOperationStep> Execute(ITree tree, OperationType operationType, int key)
    {
        var operation = OperationFactory.Create(operationType);
        return operation(tree, key);
    }
}
