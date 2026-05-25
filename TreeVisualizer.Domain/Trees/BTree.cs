using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Nodes;
using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Domain.Trees;

/// <summary>
/// B-дерево минимальной степени t.
/// Поддерживает поиск, вставку с разбиением переполненных узлов и полноценное удаление с заимствованием/слиянием.
/// </summary>
public sealed class BTree : ITree
{
    private const double VerticalStep = 105;
    private const double KeyWidth = 42;
    private const double NodeHeight = 44;
    private const double MinimumSubtreeWidth = 90;
    private const double LeafGap = 28;

    private readonly int _minDegree;
    private BTreeNode _root;

    public BTree(int minDegree = 2)
    {
        if (minDegree < 2)
            throw new ArgumentOutOfRangeException(nameof(minDegree), "Минимальная степень B-дерева должна быть не меньше 2.");

        _minDegree = minDegree;
        _root = new BTreeNode();
    }

    public string Name => "B-дерево";

    public TreeType Type => TreeType.BTree;

    public bool IsEmpty => _root.Keys.Count == 0 && _root.Children.Count == 0;

    public void Clear()
    {
        _root = new BTreeNode();
    }

    public bool Contains(int key)
    {
        return SearchNode(_root, key) is not null;
    }

    public IReadOnlyList<TreeOperationStep> Insert(int key)
    {
        var steps = new List<TreeOperationStep>
        {
            new($"Начинаем вставку ключа {key} в B-дерево.", CreateSnapshot())
        };

        BTreeNode? existingNode = SearchNode(_root, key);
        if (existingNode is not null)
        {
            steps.Add(new($"Ключ {key} уже есть в B-дереве. Повторная вставка не выполняется.", CreateSnapshot(existingNode.Id, NodeVisualState.Error)));
            return steps;
        }

        if (_root.Keys.Count == MaxKeys)
        {
            steps.Add(new("Корень переполнен. Создаем новый корень и разбиваем старый.", CreateSnapshot(_root.Id, NodeVisualState.Split)));
            var newRoot = new BTreeNode();
            newRoot.Children.Add(_root);
            SplitChild(newRoot, 0, steps);
            _root = newRoot;
            steps.Add(new("Разбиение корня завершено. Высота B-дерева увеличена на один уровень.", CreateSnapshot(_root.Id, NodeVisualState.Split)));
        }

        InsertNonFull(_root, key, steps);
        steps.Add(new($"Вставка {key} завершена.", CreateSnapshot(SearchNode(_root, key)?.Id, NodeVisualState.Inserted)));
        return steps;
    }

    public IReadOnlyList<TreeOperationStep> Delete(int key)
    {
        var steps = new List<TreeOperationStep>
        {
            new($"Начинаем удаление ключа {key} из B-дерева.", CreateSnapshot())
        };

        BTreeNode? node = SearchNode(_root, key);
        if (node is null)
        {
            steps.Add(new($"Ключ {key} не найден. Удаление невозможно.", CreateSnapshot()));
            return steps;
        }

        steps.Add(new($"Ключ {key} найден. Запускаем алгоритм удаления из B-дерева.", CreateSnapshot(node.Id, NodeVisualState.Deleted)));
        DeleteInternal(_root, key, steps);

        if (_root.Keys.Count == 0 && !_root.IsLeaf)
        {
            steps.Add(new("После удаления корень стал пустым. Первого потомка делаем новым корнем.", CreateSnapshot(_root.Id, NodeVisualState.Current)));
            _root = _root.Children[0];
        }

        if (_root.Keys.Count == 0 && _root.IsLeaf)
            _root = new BTreeNode();

        steps.Add(new($"Удаление {key} завершено. Свойства B-дерева сохранены.", CreateSnapshot()));
        return steps;
    }

    public IReadOnlyList<TreeOperationStep> Search(int key)
    {
        var steps = new List<TreeOperationStep>
        {
            new($"Начинаем поиск ключа {key} в B-дереве.", CreateSnapshot())
        };

        BTreeNode? current = _root;
        while (current is not null)
        {
            steps.Add(new($"Просматриваем узел с ключами: {FormatKeys(current)}.", CreateSnapshot(current.Id, NodeVisualState.Compared)));

            int index = FindFirstGreaterOrEqual(current, key);
            if (index < current.Keys.Count && current.Keys[index] == key)
            {
                steps.Add(new($"Ключ {key} найден в текущем узле.", CreateSnapshot(current.Id, NodeVisualState.Found)));
                return steps;
            }

            if (current.IsLeaf)
                break;

            steps.Add(new($"Ключ {key} не найден в узле. Переходим к потомку №{index + 1}.", CreateSnapshot(current.Id, NodeVisualState.Current)));
            current = current.Children[index];
        }

        steps.Add(new($"Ключ {key} не найден.", CreateSnapshot()));
        return steps;
    }

    public TreeSnapshot CreateSnapshot()
    {
        return CreateSnapshot(null, NodeVisualState.Normal);
    }

    private int MaxKeys => 2 * _minDegree - 1;

    private int MinKeys => _minDegree - 1;

    private void InsertNonFull(BTreeNode node, int key, List<TreeOperationStep> steps)
    {
        int i = node.Keys.Count - 1;
        steps.Add(new($"Работаем с узлом [{FormatKeys(node)}].", CreateSnapshot(node.Id, NodeVisualState.Current)));

        if (node.IsLeaf)
        {
            node.Keys.Add(0);
            while (i >= 0 && key < node.Keys[i])
            {
                node.Keys[i + 1] = node.Keys[i];
                i--;
            }

            node.Keys[i + 1] = key;
            steps.Add(new($"Узел является листом. Вставляем {key} в отсортированную позицию.", CreateSnapshot(node.Id, NodeVisualState.Inserted)));
            return;
        }

        while (i >= 0 && key < node.Keys[i])
            i--;
        i++;

        steps.Add(new($"Переходим к потомку №{i + 1} для вставки ключа {key}.", CreateSnapshot(node.Children[i].Id, NodeVisualState.Current)));

        if (node.Children[i].Keys.Count == MaxKeys)
        {
            steps.Add(new($"Потомок №{i + 1} переполнен. Выполняем разбиение перед спуском.", CreateSnapshot(node.Children[i].Id, NodeVisualState.Split)));
            SplitChild(node, i, steps);

            if (key > node.Keys[i])
                i++;
        }

        InsertNonFull(node.Children[i], key, steps);
    }

    private void SplitChild(BTreeNode parent, int childIndex, List<TreeOperationStep> steps)
    {
        BTreeNode fullChild = parent.Children[childIndex];
        var rightNode = new BTreeNode();
        int median = fullChild.Keys[_minDegree - 1];

        for (int j = 0; j < MinKeys; j++)
            rightNode.Keys.Add(fullChild.Keys[j + _minDegree]);

        if (!fullChild.IsLeaf)
        {
            for (int j = 0; j < _minDegree; j++)
                rightNode.Children.Add(fullChild.Children[j + _minDegree]);

            fullChild.Children.RemoveRange(_minDegree, fullChild.Children.Count - _minDegree);
        }

        fullChild.Keys.RemoveRange(_minDegree - 1, fullChild.Keys.Count - (_minDegree - 1));
        parent.Children.Insert(childIndex + 1, rightNode);
        parent.Keys.Insert(childIndex, median);

        steps.Add(new($"Средний ключ {median} поднят в родительский узел. Переполненный узел разделен на [{FormatKeys(fullChild)}] и [{FormatKeys(rightNode)}].", CreateSnapshot(parent.Id, NodeVisualState.Split)));
    }

    private void DeleteInternal(BTreeNode node, int key, List<TreeOperationStep> steps)
    {
        int index = FindFirstGreaterOrEqual(node, key);
        steps.Add(new($"Ищем ключ {key} в узле [{FormatKeys(node)}].", CreateSnapshot(node.Id, NodeVisualState.Compared)));

        if (index < node.Keys.Count && node.Keys[index] == key)
        {
            if (node.IsLeaf)
            {
                RemoveFromLeaf(node, index, steps);
            }
            else
            {
                RemoveFromInternalNode(node, index, steps);
            }

            return;
        }

        if (node.IsLeaf)
        {
            steps.Add(new($"Достигнут лист [{FormatKeys(node)}], но ключ {key} отсутствует.", CreateSnapshot(node.Id, NodeVisualState.Error)));
            return;
        }

        bool keyWouldBeInLastChild = index == node.Keys.Count;
        BTreeNode child = node.Children[index];

        if (child.Keys.Count == MinKeys)
        {
            steps.Add(new($"Перед спуском потомок №{index + 1} содержит минимум ключей. Выполняем подготовку: заимствование или слияние.", CreateSnapshot(child.Id, NodeVisualState.Current)));
            FillChild(node, index, steps);
        }

        int nextIndex = keyWouldBeInLastChild && index > node.Keys.Count
            ? index - 1
            : index;

        DeleteInternal(node.Children[nextIndex], key, steps);
    }

    private void RemoveFromLeaf(BTreeNode node, int index, List<TreeOperationStep> steps)
    {
        int removedKey = node.Keys[index];
        steps.Add(new($"Ключ {removedKey} находится в листе. Удаляем его из узла.", CreateSnapshot(node.Id, NodeVisualState.Deleted)));
        node.Keys.RemoveAt(index);
        steps.Add(new($"Из листа удален ключ {removedKey}. Текущий узел: [{FormatKeys(node)}].", CreateSnapshot(node.Id, NodeVisualState.Current)));
    }

    private void RemoveFromInternalNode(BTreeNode node, int index, List<TreeOperationStep> steps)
    {
        int key = node.Keys[index];
        BTreeNode leftChild = node.Children[index];
        BTreeNode rightChild = node.Children[index + 1];

        steps.Add(new($"Ключ {key} находится во внутреннем узле. Выбираем способ замены.", CreateSnapshot(node.Id, NodeVisualState.Deleted)));

        if (leftChild.Keys.Count >= _minDegree)
        {
            int predecessor = GetPredecessor(leftChild);
            steps.Add(new($"Левый потомок содержит достаточно ключей. Заменяем {key} предшественником {predecessor}.", CreateSnapshot(leftChild.Id, NodeVisualState.Found)));
            node.Keys[index] = predecessor;
            DeleteInternal(leftChild, predecessor, steps);
            return;
        }

        if (rightChild.Keys.Count >= _minDegree)
        {
            int successor = GetSuccessor(rightChild);
            steps.Add(new($"Правый потомок содержит достаточно ключей. Заменяем {key} преемником {successor}.", CreateSnapshot(rightChild.Id, NodeVisualState.Found)));
            node.Keys[index] = successor;
            DeleteInternal(rightChild, successor, steps);
            return;
        }

        steps.Add(new($"Оба соседних потомка имеют минимум ключей. Сливаем их через ключ {key}.", CreateSnapshot(node.Id, NodeVisualState.Split)));
        MergeChildren(node, index, steps);
        DeleteInternal(leftChild, key, steps);
    }

    private void FillChild(BTreeNode parent, int index, List<TreeOperationStep> steps)
    {
        if (index > 0 && parent.Children[index - 1].Keys.Count >= _minDegree)
        {
            BorrowFromPrevious(parent, index, steps);
            return;
        }

        if (index < parent.Keys.Count && parent.Children[index + 1].Keys.Count >= _minDegree)
        {
            BorrowFromNext(parent, index, steps);
            return;
        }

        if (index < parent.Keys.Count)
        {
            MergeChildren(parent, index, steps);
        }
        else
        {
            MergeChildren(parent, index - 1, steps);
        }
    }

    private void BorrowFromPrevious(BTreeNode parent, int childIndex, List<TreeOperationStep> steps)
    {
        BTreeNode child = parent.Children[childIndex];
        BTreeNode sibling = parent.Children[childIndex - 1];

        steps.Add(new($"Левый сосед содержит лишний ключ. Заимствуем ключ через родителя.", CreateSnapshot(sibling.Id, NodeVisualState.Current)));

        child.Keys.Insert(0, parent.Keys[childIndex - 1]);

        if (!sibling.IsLeaf)
        {
            child.Children.Insert(0, sibling.Children[^1]);
            sibling.Children.RemoveAt(sibling.Children.Count - 1);
        }

        parent.Keys[childIndex - 1] = sibling.Keys[^1];
        sibling.Keys.RemoveAt(sibling.Keys.Count - 1);

        steps.Add(new($"Заимствование слева завершено. Потомок теперь содержит [{FormatKeys(child)}].", CreateSnapshot(child.Id, NodeVisualState.Current)));
    }

    private void BorrowFromNext(BTreeNode parent, int childIndex, List<TreeOperationStep> steps)
    {
        BTreeNode child = parent.Children[childIndex];
        BTreeNode sibling = parent.Children[childIndex + 1];

        steps.Add(new($"Правый сосед содержит лишний ключ. Заимствуем ключ через родителя.", CreateSnapshot(sibling.Id, NodeVisualState.Current)));

        child.Keys.Add(parent.Keys[childIndex]);

        if (!sibling.IsLeaf)
        {
            child.Children.Add(sibling.Children[0]);
            sibling.Children.RemoveAt(0);
        }

        parent.Keys[childIndex] = sibling.Keys[0];
        sibling.Keys.RemoveAt(0);

        steps.Add(new($"Заимствование справа завершено. Потомок теперь содержит [{FormatKeys(child)}].", CreateSnapshot(child.Id, NodeVisualState.Current)));
    }

    private void MergeChildren(BTreeNode parent, int keyIndex, List<TreeOperationStep> steps)
    {
        BTreeNode leftChild = parent.Children[keyIndex];
        BTreeNode rightChild = parent.Children[keyIndex + 1];
        int separator = parent.Keys[keyIndex];

        steps.Add(new($"Сливаем потомков [{FormatKeys(leftChild)}] и [{FormatKeys(rightChild)}] через разделитель {separator}.", CreateSnapshot(parent.Id, NodeVisualState.Split)));

        leftChild.Keys.Add(separator);
        leftChild.Keys.AddRange(rightChild.Keys);

        if (!rightChild.IsLeaf)
            leftChild.Children.AddRange(rightChild.Children);

        parent.Keys.RemoveAt(keyIndex);
        parent.Children.RemoveAt(keyIndex + 1);

        steps.Add(new($"Слияние завершено. Получен узел [{FormatKeys(leftChild)}].", CreateSnapshot(leftChild.Id, NodeVisualState.Split)));
    }

    private BTreeNode? SearchNode(BTreeNode? node, int key)
    {
        if (node is null || (node.Keys.Count == 0 && node.IsLeaf))
            return null;

        int index = FindFirstGreaterOrEqual(node, key);
        if (index < node.Keys.Count && node.Keys[index] == key)
            return node;

        return node.IsLeaf ? null : SearchNode(node.Children[index], key);
    }

    private static int FindFirstGreaterOrEqual(BTreeNode node, int key)
    {
        int index = 0;
        while (index < node.Keys.Count && key > node.Keys[index])
            index++;

        return index;
    }

    private static int GetPredecessor(BTreeNode node)
    {
        BTreeNode current = node;
        while (!current.IsLeaf)
            current = current.Children[^1];

        return current.Keys[^1];
    }

    private static int GetSuccessor(BTreeNode node)
    {
        BTreeNode current = node;
        while (!current.IsLeaf)
            current = current.Children[0];

        return current.Keys[0];
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
        if (IsEmpty)
            return TreeSnapshot.Empty;

        var widths = new Dictionary<string, double>();
        var nodes = new List<VisualNode>();
        var edges = new List<VisualEdge>();

        double Measure(BTreeNode node)
        {
            double ownWidth = Math.Max(MinimumSubtreeWidth, Math.Max(1, node.Keys.Count) * KeyWidth + 24);
            if (node.IsLeaf)
            {
                widths[node.Id] = ownWidth;
                return ownWidth;
            }

            double childrenWidth = node.Children.Sum(Measure) + Math.Max(0, node.Children.Count - 1) * LeafGap;
            double width = Math.Max(ownWidth, childrenWidth);
            widths[node.Id] = width;
            return width;
        }

        void Place(BTreeNode node, double left, int depth)
        {
            double subtreeWidth = widths[node.Id];
            double ownWidth = Math.Max(MinimumSubtreeWidth, Math.Max(1, node.Keys.Count) * KeyWidth + 24);
            double x = left + subtreeWidth / 2;
            double y = 60 + depth * VerticalStep;

            NodeVisualState state = states.TryGetValue(node.Id, out NodeVisualState storedState)
                ? storedState
                : NodeVisualState.Normal;

            nodes.Add(new VisualNode(node.Id, node.Keys.ToArray(), x, y, ownWidth, NodeHeight, state));

            if (node.IsLeaf)
                return;

            double childLeft = left;
            foreach (BTreeNode child in node.Children)
            {
                edges.Add(new VisualEdge(node.Id, child.Id));
                Place(child, childLeft, depth + 1);
                childLeft += widths[child.Id] + LeafGap;
            }
        }

        Measure(_root);
        Place(_root, 40, 0);
        return new TreeSnapshot(nodes, edges);
    }

    private static string FormatKeys(BTreeNode node)
    {
        return node.Keys.Count == 0 ? "пусто" : string.Join(", ", node.Keys);
    }
}
