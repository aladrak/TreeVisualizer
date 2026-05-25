using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Visualization;
using TreeVisualizer.Infrastructure.Factories;

namespace TreeVisualizer.Infrastructure.Execution;

/// <summary>
/// Выполняет операции над текущим деревом и возвращает шаги визуализации.
/// </summary>
public sealed class OperationExecutor
{
    public IReadOnlyList<TreeOperationStep> Execute(ITree tree, OperationType operationType, int key)
    {
        Func<ITree, int, IReadOnlyList<TreeOperationStep>> operation = OperationFactory.Create(operationType);
        return operation(tree, key);
    }
}
