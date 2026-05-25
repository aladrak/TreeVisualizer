using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Infrastructure.Layout;

/// <summary>
/// Заготовка алгоритма размещения B-дерева.
/// Координаты уже формируются в доменной модели, поэтому класс оставлен как расширяемая точка архитектуры.
/// </summary>
public sealed class BTreeLayoutAlgorithm : ITreeLayoutAlgorithm
{
    public TreeSnapshot Apply(TreeSnapshot snapshot)
    {
        return snapshot;
    }
}
