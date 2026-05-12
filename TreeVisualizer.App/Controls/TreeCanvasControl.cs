using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.App.Controls;

public class TreeCanvasControl : GraphicsView
{
    public static readonly BindableProperty NodesProperty =
        BindableProperty.Create(nameof(Nodes), typeof(IReadOnlyList<VisualNode>), typeof(TreeCanvasControl), propertyChanged: OnNodesChanged);

    public IReadOnlyList<VisualNode> Nodes
    {
        get => (IReadOnlyList<VisualNode>)GetValue(NodesProperty);
        set => SetValue(NodesProperty, value);
    }

    private readonly TreeDrawable _drawable = new();

    public TreeCanvasControl()
    {
        Drawable = _drawable;
        Background = Colors.WhiteSmoke;
        HeightRequest = 550;
    }

    private static void OnNodesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TreeCanvasControl control && newValue is IReadOnlyList<VisualNode> nodes)
        {
            control._drawable.Update(nodes);
            control.Invalidate(); // Принудительная перерисовка
        }
    }

    private class TreeDrawable : IDrawable
    {
        private IReadOnlyList<VisualNode> _nodes = Array.Empty<VisualNode>();
        private const float Radius = 22f;
        private const float FontSize = 14f;

        public void Update(IReadOnlyList<VisualNode> nodes) => _nodes = nodes;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.WhiteSmoke;
            canvas.FillRectangle(dirtyRect);

            if (_nodes.Count == 0) return;

            // 1. Рисуем связи
            canvas.StrokeColor = Colors.Gray;
            canvas.StrokeSize = 2;
            foreach (var node in _nodes)
            {
                if (node.Node.Parent != null)
                {
                    var parent = _nodes.FirstOrDefault(n => n.Id == node.Node.Parent.Id);
                    if (parent != null)
                        canvas.DrawLine((float)parent.X + Radius, (float)parent.Y + Radius,
                                        (float)node.X + Radius, (float)node.Y + Radius);
                }
            }

            // 2. Рисуем узлы
            foreach (var node in _nodes)
            {
                float cx = (float)node.X + Radius;
                float cy = (float)node.Y + Radius;

                canvas.FillColor = GetColor(node.State);
                canvas.FillCircle(cx, cy, Radius);
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 1.5f;
                canvas.DrawCircle(cx, cy, Radius);

                canvas.FontColor = Colors.White;
                canvas.FontSize = FontSize;
                canvas.DrawString(node.Node.Key.ToString(), cx, cy, HorizontalAlignment.Center);
            }
        }

        private static Color GetColor(NodeState state) => state switch
        {
            NodeState.Visited => Colors.Blue,
            NodeState.Target => Colors.Green,
            NodeState.Highlight => Colors.Orange,
            NodeState.Removing => Colors.Red,
            _ => Colors.SlateBlue
        };
    }
}