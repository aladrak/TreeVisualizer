namespace TreeVisualizer.Domain.Nodes;

public class BinaryTreeNode : TreeNodeBase
{
    public BinaryTreeNode(int key) : base(key) { }

    public BinaryTreeNode? Left => Children.ElementAtOrDefault(0) as BinaryTreeNode;
    public BinaryTreeNode? Right => Children.ElementAtOrDefault(1) as BinaryTreeNode;

    public void SetLeft(BinaryTreeNode? node)
    {
        if (Left != null) RemoveChild(Left);
        if (node != null) AddChild(node);
    }

    public void SetRight(BinaryTreeNode? node)
    {
        if (Right != null) RemoveChild(Right);
        if (node != null) AddChild(node);
    }
}