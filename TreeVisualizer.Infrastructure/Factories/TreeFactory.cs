using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Trees;

namespace TreeVisualizer.Infrastructure.Factories;

// Создает дерево выбранного типа.
public static class TreeFactory
{
    public static ITree Create(TreeType treeType)
    {
        return treeType switch
        {
            TreeType.BinarySearchTree => new BinarySearchTree(),
            TreeType.AvlTree => new AvlTree(),
            TreeType.BTree => new BTree(2),
            _ => throw new NotSupportedException($"Тип дерева {treeType} не поддерживается.")
        };
    }
}
