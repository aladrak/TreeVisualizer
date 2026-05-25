using TreeVisualizer.Domain.Interfaces;

namespace TreeVisualizer.Domain.Nodes;

/// <summary>
/// Узел B-дерева. Один узел может хранить несколько отсортированных ключей.
/// </summary>
public sealed class BTreeNode : ITreeNode
{
    public BTreeNode()
    {
        Id = Guid.NewGuid().ToString("N");
    }

    public string Id { get; }

    public List<int> Keys { get; } = new();

    public List<BTreeNode> Children { get; } = new();

    public int KeyCount => Keys.Count;

    public int ChildrenCount => Children.Count;

    public bool IsLeaf => Children.Count == 0;
}
