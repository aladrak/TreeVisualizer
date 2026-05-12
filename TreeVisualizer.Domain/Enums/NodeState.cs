namespace TreeVisualizer.Domain.Enums;

public enum NodeState
{
    Default,
    Visited,      // Узел посещён
    Target,       // Выделенный узел
    Highlight,    // Подсветка пути или родителя
    Removing      // Узел помечен на удаление
}