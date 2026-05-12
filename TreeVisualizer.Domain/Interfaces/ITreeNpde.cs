using TreeVisualizer.Domain.Enums;

namespace TreeVisualizer.Domain.Interfaces;

public interface ITreeNode
{
    int Id { get; }
    int Key { get; set; }
    string? Value { get; set; }
    ITreeNode? Parent { get; set; }
    IReadOnlyList<ITreeNode> Children { get; }
    NodeState State { get; set; }
    
    void AddChild(ITreeNode child);
    void RemoveChild(ITreeNode child);
}