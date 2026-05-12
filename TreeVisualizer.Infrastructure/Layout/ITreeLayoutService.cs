using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Infrastructure.Layout;

public interface ITreeLayoutService
{
    // Вычисляет координаты для всех узлов дерева.
    IReadOnlyList<VisualNode> CalculateLayout(ITreeNode root, LayoutSettings settings);
}