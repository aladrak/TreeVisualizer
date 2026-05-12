using TreeVisualizer.Domain.Interfaces;
using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.Infrastructure.Layout;

public class BinaryTreeLayoutService : ITreeLayoutService
{
    public IReadOnlyList<VisualNode> CalculateLayout(ITreeNode root, LayoutSettings settings)
    {
        var result = new List<VisualNode>();
        if (root == null) return result;

        int leafCounter = 0;
        AssignPositions(root, 0, ref leafCounter, result, settings);
        CenterParents(root, result, settings);

        // Сдвиг в положительную область
        var minX = result.Min(n => n.X);
        if (minX < 0)
            result = result.Select(n => n with { X = n.X - minX }).ToList();

        return result.AsReadOnly();
    }

    private void AssignPositions(ITreeNode node, int depth, ref int leafCounter, 
                                 List<VisualNode> result, LayoutSettings settings)
    {
        if (node == null) return;

        var left = node.Children.ElementAtOrDefault(0);
        var right = node.Children.ElementAtOrDefault(1);

        AssignPositions(left, depth + 1, ref leafCounter, result, settings);

        double x = leafCounter * (settings.NodeWidth + settings.HorizontalGap);
        double y = depth * (settings.NodeHeight + settings.VerticalGap);
        result.Add(new VisualNode(node.Id, node, x, y));
        leafCounter++;

        AssignPositions(right, depth + 1, ref leafCounter, result, settings);
    }

    private void CenterParents(ITreeNode node, List<VisualNode> result, LayoutSettings settings)
    {
        var left = node.Children.ElementAtOrDefault(0);
        var right = node.Children.ElementAtOrDefault(1);
        if (left == null && right == null) return; // Лист

        var currentNode = result.First(n => n.Id == node.Id);
        
        if (left != null && right != null)
        {
            var leftV = result.First(n => n.Id == left.Id);
            var rightV = result.First(n => n.Id == right.Id);
            currentNode = currentNode with { X = (leftV.X + rightV.X) / 2 };
        }
        else if (left != null)
        {
            var leftV = result.First(n => n.Id == left.Id);
            currentNode = currentNode with { X = leftV.X };
        }
        else
        {
            var rightV = result.First(n => n.Id == right.Id);
            currentNode = currentNode with { X = rightV.X };
        }

        result[result.FindIndex(n => n.Id == node.Id)] = currentNode;

        CenterParents(left!, result, settings);
        CenterParents(right!, result, settings);
    }
}