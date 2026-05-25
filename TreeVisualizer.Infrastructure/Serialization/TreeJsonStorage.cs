using System.Text.Json;

namespace TreeVisualizer.Infrastructure.Serialization;

/// <summary>
/// Сервис сохранения и загрузки дерева в JSON.
/// </summary>
public sealed class TreeJsonStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public async Task SaveAsync(string fileName, TreeSaveModel model, CancellationToken cancellationToken = default)
    {
        await using FileStream stream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(stream, model, Options, cancellationToken);
    }

    public async Task<TreeSaveModel?> LoadAsync(string fileName, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(fileName))
            return null;

        await using FileStream stream = File.OpenRead(fileName);
        return await JsonSerializer.DeserializeAsync<TreeSaveModel>(stream, Options, cancellationToken);
    }
}
