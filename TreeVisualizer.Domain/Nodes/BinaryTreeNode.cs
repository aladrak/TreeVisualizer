using TreeVisualizer.Domain.Interfaces;

namespace TreeVisualizer.Domain.Nodes;

public sealed class BinaryTreeNode(int key) : ITreeNode
{
    public string Id { get; } = Guid.NewGuid().ToString("N");

    public int Key { get; set; } = key;

    public bool IsLeaf => Left is null && Right is null;

    public BinaryTreeNode? Left { get; set; }

    public BinaryTreeNode? Right { get; set; }
}
