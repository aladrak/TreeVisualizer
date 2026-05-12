using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;

namespace TreeVisualizer.Domain.Nodes;

public abstract class TreeNodeBase : ITreeNode
{
    private static int _nextId = 0;
    protected readonly List<ITreeNode> _children = new();

    public int Id { get; }
    public int Key { get; set; }
    public string? Value { get; set; }
    public ITreeNode? Parent { get; set; }
    public IReadOnlyList<ITreeNode> Children => _children.AsReadOnly();
    public NodeState State { get; set; }

    protected TreeNodeBase(int key)
    {
        Id = Interlocked.Increment(ref _nextId);
        Key = key;
    }

    public void AddChild(ITreeNode child)
    {
        if (child is null) return;
        child.Parent = this;
        _children.Add(child);
    }

    public void RemoveChild(ITreeNode child)
    {
        _children.Remove(child);
        child.Parent = null;
    }

    public override string ToString() => $"[{Id}] Key={Key}, State={State}";
}