using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Visualization;

namespace TreeVisualizer.App.Controls;

public sealed partial class TreeCanvas : GraphicsView
{
    private readonly TreeDrawable _drawable = new();

    public TreeCanvas()
    {
        Drawable = _drawable;
        BackgroundColor = Color.FromArgb("#F8FAFC");
        HeightRequest = 520;
        MinimumHeightRequest = 360;
    }

    public void SetSnapshot(TreeSnapshot snapshot)
    {
        _drawable.Snapshot = snapshot;
        Invalidate();
    }

    private sealed class TreeDrawable : IDrawable
    {
        public TreeSnapshot Snapshot { get; set; } = TreeSnapshot.Empty;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();
            canvas.FillColor = Color.FromArgb("#F8FAFC");
            canvas.FillRectangle(dirtyRect);

            if (Snapshot.Nodes.Count == 0)
            {
                DrawEmptyState(canvas, dirtyRect);
                canvas.RestoreState();
                return;
            }

            Dictionary<string, VisualNode> map = Snapshot.Nodes.ToDictionary(node => node.Id);
            RectF bounds = CalculateBounds(Snapshot.Nodes);
            float scale = CalculateScale(bounds, dirtyRect);
            float offsetX = dirtyRect.X + (dirtyRect.Width - bounds.Width * scale) / 2f - bounds.X * scale;
            float offsetY = dirtyRect.Y + 28f - bounds.Y * scale;

            PointF Transform(double x, double y)
            {
                return new PointF((float)(x * scale + offsetX), (float)(y * scale + offsetY));
            }

            DrawEdges(canvas, map, Transform);
            DrawNodes(canvas, Transform, scale);
            canvas.RestoreState();
        }

        private static void DrawEmptyState(ICanvas canvas, RectF rect)
        {
            canvas.FontSize = 22;
            canvas.FontColor = Color.FromArgb("#64748B");
            canvas.DrawString(
                "Дерево пустое. Введите число снизу и нажмите «Добавить».",
                rect,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);
        }

        private static RectF CalculateBounds(IReadOnlyList<VisualNode> nodes)
        {
            double minX = nodes.Min(node => node.X - node.Width / 2);
            double maxX = nodes.Max(node => node.X + node.Width / 2);
            double minY = nodes.Min(node => node.Y - node.Height / 2);
            double maxY = nodes.Max(node => node.Y + node.Height / 2);

            return new RectF(
                (float)minX,
                (float)minY,
                (float)Math.Max(1, maxX - minX),
                (float)Math.Max(1, maxY - minY));
        }

        private static float CalculateScale(RectF bounds, RectF dirtyRect)
        {
            float scaleX = (dirtyRect.Width - 60) / Math.Max(1, bounds.Width);
            float scaleY = (dirtyRect.Height - 80) / Math.Max(1, bounds.Height);
            return Math.Clamp(Math.Min(scaleX, scaleY), 0.45f, 1.25f);
        }

        private void DrawEdges(ICanvas canvas, Dictionary<string, VisualNode> map, Func<double, double, PointF> transform)
        {
            canvas.StrokeColor = Color.FromArgb("#94A3B8");
            canvas.StrokeSize = 2;

            foreach (VisualEdge edge in Snapshot.Edges)
            {
                if (!map.TryGetValue(edge.FromId, out VisualNode? from) || !map.TryGetValue(edge.ToId, out VisualNode? to))
                    continue;

                PointF fromPoint = transform(from.X, from.Y + from.Height / 2);
                PointF toPoint = transform(to.X, to.Y - to.Height / 2);
                canvas.DrawLine(fromPoint.X, fromPoint.Y, toPoint.X, toPoint.Y);
            }
        }

        private void DrawNodes(ICanvas canvas, Func<double, double, PointF> transform, float scale)
        {
            foreach (VisualNode node in Snapshot.Nodes)
            {
                PointF center = transform(node.X, node.Y);
                float width = (float)(node.Width * scale);
                float height = (float)(node.Height * scale);
                RectF rect = new(center.X - width / 2f, center.Y - height / 2f, width, height);

                Color fill = GetFillColor(node.State);
                Color stroke = GetStrokeColor(node.State);

                canvas.FillColor = fill;
                canvas.StrokeColor = stroke;
                canvas.StrokeSize = node.State == NodeVisualState.Normal ? 2 : 4;

                if (node.Keys.Count <= 1)
                {
                    float radius = Math.Min(width, height) / 2f;
                    canvas.FillCircle(center.X, center.Y, radius);
                    canvas.DrawCircle(center.X, center.Y, radius);
                    DrawCenteredText(canvas, rect, node.Keys.Count == 0 ? string.Empty : node.Keys[0].ToString());
                }
                else
                {
                    canvas.FillRoundedRectangle(rect, 10);
                    canvas.DrawRoundedRectangle(rect, 10);
                    DrawBTreeKeys(canvas, node, rect);
                }
            }
        }

        private static void DrawCenteredText(ICanvas canvas, RectF rect, string text)
        {
            canvas.FontColor = Color.FromArgb("#0F172A");
            canvas.FontSize = Math.Clamp(rect.Height * 0.34f, 12, 20);
            canvas.DrawString(text, rect, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        private static void DrawBTreeKeys(ICanvas canvas, VisualNode node, RectF rect)
        {
            float segmentWidth = rect.Width / Math.Max(1, node.Keys.Count);

            canvas.FontColor = Color.FromArgb("#0F172A");
            canvas.FontSize = Math.Clamp(rect.Height * 0.34f, 12, 18);

            for (int i = 0; i < node.Keys.Count; i++)
            {
                RectF segment = new(rect.X + i * segmentWidth, rect.Y, segmentWidth, rect.Height);
                canvas.DrawString(node.Keys[i].ToString(), segment, HorizontalAlignment.Center, VerticalAlignment.Center);

                if (i < node.Keys.Count - 1)
                {
                    canvas.StrokeColor = Color.FromArgb("#334155");
                    canvas.StrokeSize = 1;
                    float x = rect.X + (i + 1) * segmentWidth;
                    canvas.DrawLine(x, rect.Y, x, rect.Y + rect.Height);
                }
            }
        }

        private static Color GetFillColor(NodeVisualState state)
        {
            return state switch
            {
                NodeVisualState.Current => Color.FromArgb("#BAE6FD"),
                NodeVisualState.Compared => Color.FromArgb("#FEF08A"),
                NodeVisualState.Found => Color.FromArgb("#BBF7D0"),
                NodeVisualState.Inserted => Color.FromArgb("#86EFAC"),
                NodeVisualState.Deleted => Color.FromArgb("#FDBA74"),
                NodeVisualState.Rotated => Color.FromArgb("#DDD6FE"),
                NodeVisualState.Split => Color.FromArgb("#FED7AA"),
                NodeVisualState.Error => Color.FromArgb("#FCA5A5"),
                _ => Colors.White
            };
        }

        private static Color GetStrokeColor(NodeVisualState state)
        {
            return state switch
            {
                NodeVisualState.Current => Color.FromArgb("#0284C7"),
                NodeVisualState.Compared => Color.FromArgb("#CA8A04"),
                NodeVisualState.Found => Color.FromArgb("#16A34A"),
                NodeVisualState.Inserted => Color.FromArgb("#15803D"),
                NodeVisualState.Deleted => Color.FromArgb("#EA580C"),
                NodeVisualState.Rotated => Color.FromArgb("#7C3AED"),
                NodeVisualState.Split => Color.FromArgb("#C2410C"),
                NodeVisualState.Error => Color.FromArgb("#DC2626"),
                _ => Color.FromArgb("#334155")
            };
        }
    }
}
