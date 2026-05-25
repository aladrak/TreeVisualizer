using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Domain.Interfaces;

/// <summary>
/// Базовый интерфейс дерева, с которым работает визуализатор.
/// </summary>
public interface ITree
{
    string Name { get; }

    TreeType Type { get; }

    bool IsEmpty { get; }

    void Clear();

    bool Contains(int key);

    IReadOnlyList<TreeOperationStep> Insert(int key);

    IReadOnlyList<TreeOperationStep> Delete(int key);

    IReadOnlyList<TreeOperationStep> Search(int key);

    TreeSnapshot CreateSnapshot();
}
