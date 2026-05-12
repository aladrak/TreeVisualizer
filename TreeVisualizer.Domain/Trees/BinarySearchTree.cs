using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Nodes;

namespace TreeVisualizer.Domain.Trees;

public class BinarySearchTree : TreeBase
{
    public override void Insert(int key)
    {
        if (Root == null) Root = new BinaryTreeNode(key);
        else InsertRecursive((BinaryTreeNode)Root!, key);
        Count++;
    }

    private void InsertRecursive(BinaryTreeNode node, int key)
    {
        if (key < node.Key)
        {
            if (node.Left == null) node.SetLeft(new BinaryTreeNode(key));
            else InsertRecursive(node.Left, key);
        }
        else if (key > node.Key)
        {
            if (node.Right == null) node.SetRight(new BinaryTreeNode(key));
            else InsertRecursive(node.Right, key);
        }
    }

    public override void Delete(int key)
    {
        Root = DeleteRecursive((BinaryTreeNode)Root!, key);
        if (Root != null) Count--;
    }

    private BinaryTreeNode? DeleteRecursive(BinaryTreeNode node, int key)
    {
        if (node == null) return null;
        if (key < node.Key) node.SetLeft(DeleteRecursive(node.Left!, key));
        else if (key > node.Key) node.SetRight(DeleteRecursive(node.Right!, key));
        else
        {
            if (node.Left == null) return node.Right as BinaryTreeNode;
            if (node.Right == null) return node.Left as BinaryTreeNode;
            node.Key = GetMin(node.Right!);
            node.SetRight(DeleteRecursive(node.Right!, node.Key));
        }
        return node;
    }

    private int GetMin(BinaryTreeNode n) => n.Left == null ? n.Key : GetMin(n.Left);

    public override ITreeNode? Find(int key) => FindRecursive((BinaryTreeNode)Root!, key);
    private BinaryTreeNode? FindRecursive(BinaryTreeNode n, int k) =>
        n == null || n.Key == k ? n : (k < n.Key ? FindRecursive(n.Left!, k) : FindRecursive(n.Right!, k));

    public override IEnumerable<int> Traverse(TraversalType type)
    {
        var list = new List<int>();
        TraverseRecursive(Root, type, list);
        return list;
    }

    private void TraverseRecursive(ITreeNode? node, TraversalType type, List<int> list)
    {
        if (node == null) return;
        var bn = (BinaryTreeNode)node;
        switch (type)
        {
            case TraversalType.PreOrder:
                list.Add(node.Key); TraverseRecursive(bn.Left, type, list); TraverseRecursive(bn.Right, type, list); break;
            case TraversalType.InOrder:
                TraverseRecursive(bn.Left, type, list); list.Add(node.Key); TraverseRecursive(bn.Right, type, list); break;
            case TraversalType.PostOrder:
                TraverseRecursive(bn.Left, type, list); TraverseRecursive(bn.Right, type, list); list.Add(node.Key); break;
        }
    }
}