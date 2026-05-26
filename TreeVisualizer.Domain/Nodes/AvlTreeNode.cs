using TreeVisualizer.Domain.Interfaces;

namespace TreeVisualizer.Domain.Nodes;

public sealed class AvlTreeNode(int key) : ITreeNode
{
    public string Id { get; } = Guid.NewGuid().ToString("N");

    public int Key { get; set; } = key;

    public int Height { get; set; } = 1;

    public int BalanceFactor => (Left?.Height ?? 0) - (Right?.Height ?? 0);

    public bool IsLeaf => Left is null && Right is null;

    public AvlTreeNode? Left { get; set; }

    public AvlTreeNode? Right { get; set; }
}
