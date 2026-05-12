using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Trees;

namespace TreeVisualizer.Infrastructure.Factories;

public class TreeFactory
{
    public ITree CreateTree(string type) => type.ToUpperInvariant() switch
    {
        "BST" or "BINARYSEARCHTREE" => new BinarySearchTree(),
        _ => throw new ArgumentException($"Неподдерживаемый тип дерева: {type}")
    };
}