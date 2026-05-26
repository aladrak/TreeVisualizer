using System.ComponentModel;
using System.Runtime.CompilerServices;
using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Infrastructure.Factories;

namespace TreeVisualizer.App.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    public MainViewModel()
    {
        TreeTypes = new List<TreeTypeViewModel>
        {
            new(TreeType.BinarySearchTree, "Простое бинарное дерево"),
            new(TreeType.AvlTree, "Сбалансированное AVL-дерево"),
            new(TreeType.BTree, "B-дерево")
        };

        SelectedTreeType = TreeTypes[0];
        CurrentTree = TreeFactory.Create(SelectedTreeType.Type);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<TreeTypeViewModel> TreeTypes { get; }

    public TreeTypeViewModel SelectedTreeType { get; private set; }

    public ITree CurrentTree { get; private set; }

    public string Status
    {
        get;
        private set
        {
            if (field == value)
                return;

            field = value;
            OnPropertyChanged();
        }
    } = "Выбран тип дерева: простое бинарное дерево.";

    public void SelectTree(TreeTypeViewModel treeType)
    {
        SelectedTreeType = treeType;
        CurrentTree = TreeFactory.Create(treeType.Type);
        Status = $"Выбран тип дерева: {treeType.Title}.";
        OnPropertyChanged(nameof(CurrentTree));
        OnPropertyChanged(nameof(SelectedTreeType));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
