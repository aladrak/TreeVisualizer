using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Infrastructure.Layout;

/// <summary>
/// Заготовка алгоритма размещения бинарных деревьев.
/// Координаты уже формируются в доменной модели, поэтому класс оставлен как расширяемая точка архитектуры.
/// </summary>
public sealed class BinaryTreeLayoutAlgorithm : ITreeLayoutAlgorithm
{
    public TreeSnapshot Apply(TreeSnapshot snapshot)
    {
        return snapshot;
    }
}
