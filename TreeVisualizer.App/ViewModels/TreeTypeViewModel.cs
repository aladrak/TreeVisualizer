using TreeVisualizer.Domain.Enums;

namespace TreeVisualizer.App.ViewModels;

public sealed class TreeTypeViewModel(TreeType type, string title)
{
    public TreeType Type { get; } = type;

    public string Title { get; } = title;

    public override string ToString()
    {
        return Title;
    }
}
