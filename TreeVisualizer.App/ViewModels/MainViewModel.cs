using System.ComponentModel;
using TreeVisualizer.Domain;
using System.Runtime.CompilerServices;
using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Infrastructure.Factories;

namespace TreeVisualizer.App.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private ITree _currentTree;
    private string _status = "Выбран тип дерева: простое бинарное дерево.";

    public MainViewModel()
    {
        TreeTypes = new List<TreeTypeViewModel>
        {
            new(TreeType.BinarySearchTree, "Простое бинарное дерево"),
            new(TreeType.AvlTree, "Сбалансированное AVL-дерево"),
            new(TreeType.BTree, "B-дерево")
        };

        SelectedTreeType = TreeTypes[0];
        _currentTree = TreeFactory.Create(SelectedTreeType.Type);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<TreeTypeViewModel> TreeTypes { get; }

    public TreeTypeViewModel SelectedTreeType { get; private set; }

    public ITree CurrentTree => _currentTree;

    public string Status
    {
        get => _status;
        set
        {
            if (_status == value)
                return;

            _status = value;
            OnPropertyChanged();
        }
    }

    public void SelectTree(TreeTypeViewModel treeType)
    {
        SelectedTreeType = treeType;
        _currentTree = TreeFactory.Create(treeType.Type);
        Status = $"Выбран тип дерева: {treeType.Title}.";
        OnPropertyChanged(nameof(CurrentTree));
        OnPropertyChanged(nameof(SelectedTreeType));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
