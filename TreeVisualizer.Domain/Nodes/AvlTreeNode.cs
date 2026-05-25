using TreeVisualizer.Domain.Interfaces;

namespace TreeVisualizer.Domain.Nodes;

/// <summary>
/// Узел AVL-дерева.
/// </summary>
public sealed class AvlTreeNode : ITreeNode
{
    public AvlTreeNode(int key)
    {
        Key = key;
        Height = 1;
        Id = Guid.NewGuid().ToString("N");
    }

    public string Id { get; }

    public int Key { get; set; }

    public int Height { get; set; }

    public int BalanceFactor => (Left?.Height ?? 0) - (Right?.Height ?? 0);

    public bool IsLeaf => Left is null && Right is null;

    public AvlTreeNode? Left { get; set; }

    public AvlTreeNode? Right { get; set; }
}
