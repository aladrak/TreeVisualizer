using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Infrastructure.Factories;

public static class OperationFactory
{
    public static Func<ITree, int, IReadOnlyList<TreeOperationStep>> Create(OperationType operationType)
    {
        return operationType switch
        {
            OperationType.Insert => (tree, key) => tree.Insert(key),
            OperationType.Delete => (tree, key) => tree.Delete(key),
            OperationType.Search => (tree, key) => tree.Search(key),
            _ => throw new NotSupportedException($"Операция {operationType} не поддерживается.")
        };
    }
}
