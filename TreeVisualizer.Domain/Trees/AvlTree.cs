using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Nodes;
using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Domain.Trees;

/// <summary>
/// Сбалансированное дерево поиска на основе AVL-алгоритма.
/// </summary>
public sealed class AvlTree : ITree
{
    private const double HorizontalStep = 95;
    private const double VerticalStep = 95;
    private const double NodeRadius = 34;

    private AvlTreeNode? _root;

    public string Name => "Сбалансированное AVL-дерево";

    public TreeType Type => TreeType.AvlTree;

    public bool IsEmpty => _root is null;

    public void Clear()
    {
        _root = null;
    }

    public bool Contains(int key)
    {
        return FindNode(key) is not null;
    }

    public IReadOnlyList<TreeOperationStep> Insert(int key)
    {
        var steps = new List<TreeOperationStep>
        {
            new($"Начинаем вставку ключа {key} в AVL-дерево.", CreateSnapshot())
        };

        AvlTreeNode? existingNode = FindNode(key);
        if (existingNode is not null)
        {
            steps.Add(new($"Ключ {key} уже существует. Вставка отменена.", CreateSnapshot(existingNode.Id, NodeVisualState.Error)));
            return steps;
        }

        _root = InsertNode(_root, key, steps);
        steps.Add(new($"Вставка {key} завершена. AVL-дерево сбалансировано.", CreateSnapshot(FindNode(key)?.Id, NodeVisualState.Inserted)));
        return steps;
    }

    public IReadOnlyList<TreeOperationStep> Delete(int key)
    {
        var steps = new List<TreeOperationStep>
        {
            new($"Начинаем удаление ключа {key} из AVL-дерева.", CreateSnapshot())
        };

        AvlTreeNode? node = FindNode(key);
        if (node is null)
        {
            steps.Add(new($"Ключ {key} не найден. Удаление невозможно.", CreateSnapshot()));
            return steps;
        }

        steps.Add(new($"Ключ {key} найден. Выполняем стандартное удаление из дерева поиска.", CreateSnapshot(node.Id, NodeVisualState.Deleted)));
        _root = DeleteNode(_root, key, steps);
        steps.Add(new($"Удаление {key} завершено. Высоты пересчитаны, баланс восстановлен.", CreateSnapshot()));
        return steps;
    }

    public IReadOnlyList<TreeOperationStep> Search(int key)
    {
        var steps = new List<TreeOperationStep>
        {
            new($"Начинаем поиск ключа {key} в AVL-дереве.", CreateSnapshot())
        };

        AvlTreeNode? current = _root;
        while (current is not null)
        {
            steps.Add(new($"Сравниваем {key} с узлом {current.Key}.", CreateSnapshot(current.Id, NodeVisualState.Compared)));

            if (current.Key == key)
            {
                steps.Add(new($"Ключ {key} найден.", CreateSnapshot(current.Id, NodeVisualState.Found)));
                return steps;
            }

            if (key < current.Key)
            {
                steps.Add(new($"{key} меньше {current.Key}. Переходим в левое поддерево.", CreateSnapshot(current.Id, NodeVisualState.Current)));
                current = current.Left;
            }
            else
            {
                steps.Add(new($"{key} больше {current.Key}. Переходим в правое поддерево.", CreateSnapshot(current.Id, NodeVisualState.Current)));
                current = current.Right;
            }
        }

        steps.Add(new($"Ключ {key} не найден.", CreateSnapshot()));
        return steps;
    }

    public TreeSnapshot CreateSnapshot()
    {
        return CreateSnapshot(null, NodeVisualState.Normal);
    }

    private AvlTreeNode InsertNode(AvlTreeNode? node, int key, List<TreeOperationStep> steps)
    {
        if (node is null)
        {
            var newNode = new AvlTreeNode(key);
            steps.Add(new($"Свободная позиция найдена. Создаем новый узел {key}.", CreateSnapshot()));
            return newNode;
        }

        steps.Add(new($"Сравниваем {key} с узлом {node.Key}.", CreateSnapshot(node.Id, NodeVisualState.Compared)));

        if (key < node.Key)
        {
            steps.Add(new($"{key} меньше {node.Key}. Продолжаем вставку в левом поддереве.", CreateSnapshot(node.Id, NodeVisualState.Current)));
            node.Left = InsertNode(node.Left, key, steps);
        }
        else
        {
            steps.Add(new($"{key} больше {node.Key}. Продолжаем вставку в правом поддереве.", CreateSnapshot(node.Id, NodeVisualState.Current)));
            node.Right = InsertNode(node.Right, key, steps);
        }

        return Rebalance(node, steps, "вставки");
    }

    private AvlTreeNode? DeleteNode(AvlTreeNode? node, int key, List<TreeOperationStep> steps)
    {
        if (node is null)
            return null;

        steps.Add(new($"Сравниваем удаляемый ключ {key} с узлом {node.Key}.", CreateSnapshot(node.Id, NodeVisualState.Compared)));

        if (key < node.Key)
        {
            steps.Add(new($"{key} меньше {node.Key}. Идем в левое поддерево.", CreateSnapshot(node.Id, NodeVisualState.Current)));
            node.Left = DeleteNode(node.Left, key, steps);
        }
        else if (key > node.Key)
        {
            steps.Add(new($"{key} больше {node.Key}. Идем в правое поддерево.", CreateSnapshot(node.Id, NodeVisualState.Current)));
            node.Right = DeleteNode(node.Right, key, steps);
        }
        else
        {
            steps.Add(new($"Удаляем узел {node.Key}.", CreateSnapshot(node.Id, NodeVisualState.Deleted)));

            if (node.Left is null && node.Right is null)
            {
                steps.Add(new($"Узел {node.Key} является листом. Удаляем его без замены.", CreateSnapshot(node.Id, NodeVisualState.Deleted)));
                return null;
            }

            if (node.Left is null || node.Right is null)
            {
                AvlTreeNode replacement = node.Left ?? node.Right!;
                steps.Add(new($"У узла {node.Key} один потомок. Заменяем удаляемый узел потомком {replacement.Key}.", CreateSnapshot(replacement.Id, NodeVisualState.Current)));
                return replacement;
            }

            AvlTreeNode successor = FindMin(node.Right);
            steps.Add(new($"У узла {node.Key} два потомка. Берем преемника {successor.Key} из правого поддерева.", CreateSnapshot(successor.Id, NodeVisualState.Found)));
            node.Key = successor.Key;
            node.Right = DeleteNode(node.Right, successor.Key, steps);
        }

        return Rebalance(node, steps, "удаления");
    }

    private AvlTreeNode Rebalance(AvlTreeNode node, List<TreeOperationStep> steps, string reason)
    {
        UpdateHeight(node);
        int balance = GetBalance(node);
        NodeVisualState state = Math.Abs(balance) > 1 ? NodeVisualState.Error : NodeVisualState.Current;
        steps.Add(new($"После {reason} пересчитываем высоту узла {node.Key}. Коэффициент баланса: {balance}.", CreateSnapshot(node.Id, state)));

        if (balance > 1)
        {
            if (GetBalance(node.Left) < 0)
            {
                steps.Add(new($"Обнаружен случай LR у узла {node.Key}. Сначала выполняем левый поворот левого поддерева.", CreateSnapshot(node.Left?.Id, NodeVisualState.Rotated)));
                node.Left = RotateLeft(node.Left ?? throw new InvalidOperationException("LR-поворот невозможен без левого потомка."));
            }

            steps.Add(new($"Выполняем правый поворот вокруг узла {node.Key}.", CreateSnapshot(node.Id, NodeVisualState.Rotated)));
            return RotateRight(node);
        }

        if (balance < -1)
        {
            if (GetBalance(node.Right) > 0)
            {
                steps.Add(new($"Обнаружен случай RL у узла {node.Key}. Сначала выполняем правый поворот правого поддерева.", CreateSnapshot(node.Right?.Id, NodeVisualState.Rotated)));
                node.Right = RotateRight(node.Right ?? throw new InvalidOperationException("RL-поворот невозможен без правого потомка."));
            }

            steps.Add(new($"Выполняем левый поворот вокруг узла {node.Key}.", CreateSnapshot(node.Id, NodeVisualState.Rotated)));
            return RotateLeft(node);
        }

        return node;
    }

    private AvlTreeNode? FindNode(int key)
    {
        AvlTreeNode? current = _root;
        while (current is not null)
        {
            if (current.Key == key)
                return current;

            current = key < current.Key ? current.Left : current.Right;
        }

        return null;
    }

    private static AvlTreeNode FindMin(AvlTreeNode node)
    {
        AvlTreeNode current = node;
        while (current.Left is not null)
            current = current.Left;

        return current;
    }

    private static int Height(AvlTreeNode? node)
    {
        return node?.Height ?? 0;
    }

    private static void UpdateHeight(AvlTreeNode node)
    {
        node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;
    }

    private static int GetBalance(AvlTreeNode? node)
    {
        return node is null ? 0 : Height(node.Left) - Height(node.Right);
    }

    private static AvlTreeNode RotateRight(AvlTreeNode y)
    {
        AvlTreeNode x = y.Left ?? throw new InvalidOperationException("Правый поворот невозможен без левого потомка.");
        AvlTreeNode? transferSubtree = x.Right;

        x.Right = y;
        y.Left = transferSubtree;

        UpdateHeight(y);
        UpdateHeight(x);

        return x;
    }

    private static AvlTreeNode RotateLeft(AvlTreeNode x)
    {
        AvlTreeNode y = x.Right ?? throw new InvalidOperationException("Левый поворот невозможен без правого потомка.");
        AvlTreeNode? transferSubtree = y.Left;

        y.Left = x;
        x.Right = transferSubtree;

        UpdateHeight(x);
        UpdateHeight(y);

        return y;
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

        void Assign(AvlTreeNode? node, int depth)
        {
            if (node is null)
                return;

            Assign(node.Left, depth + 1);
            positions[node.Id] = (70 + index * HorizontalStep, 60 + depth * VerticalStep);
            index++;
            Assign(node.Right, depth + 1);
        }

        void Collect(AvlTreeNode? node)
        {
            if (node is null)
                return;

            var position = positions[node.Id];
            NodeVisualState state = states.TryGetValue(node.Id, out NodeVisualState storedState)
                ? storedState
                : NodeVisualState.Normal;

            nodes.Add(new VisualNode(node.Id, new[] { node.Key }, position.X, position.Y, NodeRadius * 2, NodeRadius * 2, state));

            if (node.Left is not null)
            {
                edges.Add(new VisualEdge(node.Id, node.Left.Id));
                Collect(node.Left);
            }

            if (node.Right is not null)
            {
                edges.Add(new VisualEdge(node.Id, node.Right.Id));
                Collect(node.Right);
            }
        }

        Assign(_root, 0);
        Collect(_root);
        return new TreeSnapshot(nodes, edges);
    }
}
