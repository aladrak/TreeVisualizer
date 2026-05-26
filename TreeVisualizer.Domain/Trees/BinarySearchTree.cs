using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Nodes;
using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Domain.Trees;

// Простое бинарное дерево поиска.
public sealed class BinarySearchTree : ITree
{
    private const double HorizontalStep = 90;
    private const double VerticalStep = 95;
    private const double NodeRadius = 34;

    private BinaryTreeNode? _root;

    public string Name => "Простое бинарное дерево";

    public TreeType Type => TreeType.BinarySearchTree;

    public bool IsEmpty => _root is null;

    public void Clear()
    {
        _root = null;
    }

    public bool Contains(int key)
    {
        BinaryTreeNode? current = _root;
        while (current is not null)
        {
            if (current.Key == key)
                return true;

            current = key < current.Key ? current.Left : current.Right;
        }

        return false;
    }

    public IReadOnlyList<TreeOperationStep> Insert(int key)
    {
        var steps = new List<TreeOperationStep>
        {
            new($"Начинаем вставку ключа {key}.", CreateSnapshot())
        };

        if (_root is null)
        {
            _root = new BinaryTreeNode(key);
            steps.Add(new($"Дерево пустое. Ключ {key} становится корнем.", CreateSnapshot(_root.Id, NodeVisualState.Inserted)));
            return steps;
        }

        BinaryTreeNode current = _root;
        while (true)
        {
            steps.Add(new($"Сравниваем {key} с узлом {current.Key}.", CreateSnapshot(current.Id, NodeVisualState.Compared)));

            if (key == current.Key)
            {
                steps.Add(new($"Ключ {key} уже есть в дереве. Повторная вставка не выполняется.", CreateSnapshot(current.Id, NodeVisualState.Error)));
                return steps;
            }

            if (key < current.Key)
            {
                steps.Add(new($"{key} меньше {current.Key}. Переходим в левое поддерево.", CreateSnapshot(current.Id, NodeVisualState.Current)));
                if (current.Left is null)
                {
                    current.Left = new BinaryTreeNode(key);
                    steps.Add(new($"Свободное место найдено. Вставляем {key} слева от {current.Key}.", CreateSnapshot(current.Left.Id, NodeVisualState.Inserted)));
                    return steps;
                }

                current = current.Left;
            }
            else
            {
                steps.Add(new($"{key} больше {current.Key}. Переходим в правое поддерево.", CreateSnapshot(current.Id, NodeVisualState.Current)));
                if (current.Right is null)
                {
                    current.Right = new BinaryTreeNode(key);
                    steps.Add(new($"Свободное место найдено. Вставляем {key} справа от {current.Key}.", CreateSnapshot(current.Right.Id, NodeVisualState.Inserted)));
                    return steps;
                }

                current = current.Right;
            }
        }
    }

    public IReadOnlyList<TreeOperationStep> Delete(int key)
    {
        var steps = new List<TreeOperationStep>
        {
            new($"Начинаем удаление ключа {key}.", CreateSnapshot())
        };

        BinaryTreeNode? parent = null;
        BinaryTreeNode? current = _root;

        while (current is not null && current.Key != key)
        {
            steps.Add(new($"Проверяем узел {current.Key}.", CreateSnapshot(current.Id, NodeVisualState.Compared)));
            parent = current;
            current = key < current.Key ? current.Left : current.Right;
        }

        if (current is null)
        {
            steps.Add(new($"Ключ {key} не найден. Удаление невозможно.", CreateSnapshot()));
            return steps;
        }

        steps.Add(new($"Узел {key} найден. Подготавливаем удаление.", CreateSnapshot(current.Id, NodeVisualState.Deleted)));

        if (current.Left is not null && current.Right is not null)
        {
            BinaryTreeNode successorParent = current;
            BinaryTreeNode successor = current.Right;
            while (successor.Left is not null)
            {
                steps.Add(new($"Ищем минимальный узел в правом поддереве. Текущий кандидат: {successor.Key}.", CreateSnapshot(successor.Id, NodeVisualState.Current)));
                successorParent = successor;
                successor = successor.Left;
            }

            steps.Add(new($"Заменяем значение {current.Key} на преемника {successor.Key}.", CreateSnapshot(successor.Id, NodeVisualState.Found)));
            current.Key = successor.Key;
            parent = successorParent;
            current = successor;
        }

        BinaryTreeNode? child = current.Left ?? current.Right;

        if (parent is null)
        {
            _root = child;
        }
        else if (parent.Left == current)
        {
            parent.Left = child;
        }
        else
        {
            parent.Right = child;
        }

        steps.Add(new($"Ключ {key} удален. Дерево перестроено.", CreateSnapshot()));
        return steps;
    }

    public IReadOnlyList<TreeOperationStep> Search(int key)
    {
        var steps = new List<TreeOperationStep>
        {
            new($"Начинаем поиск ключа {key}.", CreateSnapshot())
        };

        BinaryTreeNode? current = _root;
        while (current is not null)
        {
            steps.Add(new($"Сравниваем {key} с узлом {current.Key}.", CreateSnapshot(current.Id, NodeVisualState.Compared)));

            if (current.Key == key)
            {
                steps.Add(new($"Ключ {key} найден.", CreateSnapshot(current.Id, NodeVisualState.Found)));
                return steps;
            }

            current = key < current.Key ? current.Left : current.Right;
        }

        steps.Add(new($"Ключ {key} не найден.", CreateSnapshot()));
        return steps;
    }

    public TreeSnapshot CreateSnapshot()
    {
        return CreateSnapshot(null, NodeVisualState.Normal);
    }

    private TreeSnapshot CreateSnapshot(string? highlightedNodeId, NodeVisualState highlightedState)
    {
        var states = new Dictionary<string, NodeVisualState>();
        if (!string.IsNullOrWhiteSpace(highlightedNodeId))
            states[highlightedNodeId] = highlightedState;

        return BuildSnapshot(states);
    }

    private TreeSnapshot BuildSnapshot(IReadOnlyDictionary<string, NodeVisualState> states)
    {
        if (_root is null)
            return TreeSnapshot.Empty;

        var nodes = new List<VisualNode>();
        var edges = new List<VisualEdge>();
        var positions = new Dictionary<string, (double X, double Y)>();
        int index = 0;

        void Assign(BinaryTreeNode? node, int depth)
        {
            if (node is null)
                return;

            Assign(node.Left, depth + 1);
            positions[node.Id] = (70 + index * HorizontalStep, 60 + depth * VerticalStep);
            index++;
            Assign(node.Right, depth + 1);
        }

        void Collect(BinaryTreeNode? node)
        {
            if (node is null)
                return;

            var position = positions[node.Id];
            NodeVisualState state = states.GetValueOrDefault(node.Id, NodeVisualState.Normal);

            nodes.Add(new VisualNode(node.Id, [node.Key], position.X, position.Y, NodeRadius * 2, NodeRadius * 2, state));

            if (node.Left is not null)
            {
                edges.Add(new VisualEdge(node.Id, node.Left.Id));
                Collect(node.Left);
            }

            if (node.Right is null) return;
            edges.Add(new VisualEdge(node.Id, node.Right.Id));
            Collect(node.Right);
        }

        Assign(_root, 0);
        Collect(_root);
        return new TreeSnapshot(nodes, edges);
    }
}
