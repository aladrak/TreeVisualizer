using TreeVisualizer.Domain.Enums;

namespace TreeVisualizer.App.ViewModels;

/// <summary>
/// Элемент списка выбора дерева.
/// </summary>
public sealed class TreeTypeViewModel
{
    public TreeTypeViewModel(TreeType type, string title)
    {
        Type = type;
        Title = title;
    }

    public TreeType Type { get; }

    public string Title { get; }

    public override string ToString()
    {
        return Title;
    }
}
