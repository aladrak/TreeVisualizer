using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Domain.Interfaces;

/// <summary>
/// Интерфейс операции, представленной набором визуальных шагов.
/// </summary>
public interface ITreeOperation
{
    IReadOnlyList<TreeOperationStep> Execute(int key);
}
