using TreeVisualizer.Domain.Interfaces;

namespace TreeVisualizer.Infrastructure.Execution;

public interface ITreeOperationExecutor
{
    // Выполняет операцию над деревом и возвращает последовательность шагов для визуализации.
    IEnumerable<OperationStep> Execute(ITree tree, Action<ITree> operation);
}

