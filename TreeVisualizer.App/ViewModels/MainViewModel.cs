using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Visualization;
using TreeVisualizer.Infrastructure.Factories;
using TreeVisualizer.Infrastructure.Layout;

namespace TreeVisualizer.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITreeLayoutService _layoutService;
    private readonly TreeFactory _treeFactory;

    [ObservableProperty] private IReadOnlyList<VisualNode> _visualNodes = Array.Empty<VisualNode>();
    [ObservableProperty] private int _inputValue = 0;
    [ObservableProperty] private string _statusMessage = "Готово к работе";

    public ITree CurrentTree { get; private set; }

    public MainViewModel(ITreeLayoutService layoutService, TreeFactory treeFactory)
    {
        _layoutService = layoutService;
        _treeFactory = treeFactory;
        CurrentTree = _treeFactory.CreateTree("BST");
        RefreshLayout();
    }

    [RelayCommand] private void InsertNode()
    {
        if (InputValue == 0) return;
        CurrentTree.Insert(InputValue);
        RefreshLayout();
        StatusMessage = $"Вставлен: {InputValue}";
        InputValue = 0;
    }

    [RelayCommand] private void DeleteNode()
    {
        if (InputValue == 0) return;
        CurrentTree.Delete(InputValue);
        RefreshLayout();
        StatusMessage = $"Удалён: {InputValue}";
        InputValue = 0;
    }

    [RelayCommand] private void ClearTree()
    {
        CurrentTree.Clear();
        RefreshLayout();
        StatusMessage = "Дерево очищено";
    }

    private void RefreshLayout()
    {
        var settings = new LayoutSettings();
        VisualNodes = _layoutService.CalculateLayout(CurrentTree.Root, settings);
    }
}