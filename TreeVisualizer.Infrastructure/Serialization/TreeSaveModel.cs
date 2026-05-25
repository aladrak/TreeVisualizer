using TreeVisualizer.Domain.Enums;

namespace TreeVisualizer.Infrastructure.Serialization;

/// <summary>
/// Модель для сохранения набора ключей дерева.
/// </summary>
public sealed class TreeSaveModel
{
    public TreeType TreeType { get; set; }

    public List<int> Keys { get; set; } = new();
}
