namespace TreeVisualizer.Domain.Interfaces;

public interface ITree
{
    ITreeNode? Root { get; set; }
    int Count { get; }
    
    void Insert(int key);
    void Delete(int key);
    ITreeNode? Find(int key);
    IEnumerable<int> Traverse(Domain.Enums.TraversalType type);
    void Clear();
}