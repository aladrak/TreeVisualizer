using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;

namespace TreeVisualizer.Domain.Trees;

public abstract class TreeBase : ITree
{
    public ITreeNode? Root { get; set; }
    public int Count { get; protected set; }

    public abstract void Insert(int key);
    public abstract void Delete(int key);
    public abstract ITreeNode? Find(int key);
    public abstract IEnumerable<int> Traverse(TraversalType type);

    public void Clear()
    {
        Root = null;
        Count = 0;
    }
}