using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Domain.Interfaces;

/// <summary>
/// Интерфейс объекта, который может сформировать снимок для графической отрисовки.
/// </summary>
public interface IVisualizableTree
{
    TreeSnapshot CreateSnapshot();
}
