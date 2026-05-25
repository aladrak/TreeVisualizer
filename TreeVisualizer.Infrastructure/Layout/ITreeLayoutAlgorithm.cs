using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Infrastructure.Layout;

/// <summary>
/// Общий интерфейс алгоритмов размещения узлов на плоскости.
/// </summary>
public interface ITreeLayoutAlgorithm
{
    TreeSnapshot Apply(TreeSnapshot snapshot);
}
