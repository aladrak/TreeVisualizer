using TreeVisualizer.Domain.Interfaces;

namespace TreeVisualizer.Domain.Nodes;

public sealed class BinaryTreeNode : ITreeNode
{
    public BinaryTreeNode(int key)
    {
        Key = key;
        Id = Guid.NewGuid().ToString("N");
    }

    public string Id { get; }

    public int Key { get; set; }

    public bool IsLeaf => Left is null && Right is null;

    public BinaryTreeNode? Left { get; set; }

    public BinaryTreeNode? Right { get; set; }
}
