namespace TreeVisualizer.Domain.Enums;

/// <summary>
/// Визуальное состояние узла при пошаговой демонстрации алгоритма.
/// </summary>
public enum NodeVisualState
{
    Normal,
    Current,
    Compared,
    Found,
    Inserted,
    Deleted,
    Rotated,
    Split,
    Error
}
