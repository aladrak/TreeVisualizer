using System.Collections;
using Microsoft.Maui.Controls.Shapes;
using TreeVisualizer.App.Controls;
using TreeVisualizer.App.Services;
using TreeVisualizer.App.ViewModels;
using TreeVisualizer.Domain.Enums;
using TreeVisualizer.Domain.Visualization;
using TreeVisualizer.Infrastructure.Execution;

namespace TreeVisualizer.App.Views;

public sealed partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel = new();
    private readonly OperationExecutor _operationExecutor = new();
    private readonly StepPlayer _stepPlayer = new();
    private readonly AnimationService _animationService = new();
    private readonly DialogService _dialogService = new();

    private readonly TreeCanvas _treeCanvas = new();
    private readonly Picker _treePicker = new();
    private readonly Entry _valueEntry = new();
    private readonly Label _statusLabel = new();
    private readonly Label _stepLabel = new();
    private readonly Button _nextStepButton = new();
    private readonly Button _autoButton = new();

    private IDispatcherTimer? _timer;
    private bool _isAutoPlaying;

    public MainPage()
    {
        Title = "TreeVisualizer";
        BackgroundColor = Color.FromArgb("#CBD5E1");
        BindingContext = _viewModel;

        BuildInterface();
        ResetVisualization();
    }

    private void BuildInterface()
    {
        var root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto }
            },
            RowSpacing = 0
        };

        View topPanel = BuildTopPanel();
        View centerPanel = BuildCenterPanel();
        View bottomPanel = BuildBottomPanel();

        root.Add(topPanel, 0, 0);
        root.Add(centerPanel, 0, 1);
        root.Add(bottomPanel, 0, 2);

        Content = root;
    }

    private View BuildTopPanel()
    {
        var panel = new TreeToolbar
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = new GridLength(360) },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        var titleLabel = new Label
        {
            Text = "Вид дерева:",
            FontAttributes = FontAttributes.Bold,
            FontSize = 16,
            TextColor = Colors.White,
            VerticalTextAlignment = TextAlignment.Center
        };

        _treePicker.Title = "Выберите дерево";
        _treePicker.ItemsSource = (IList)_viewModel.TreeTypes;
        _treePicker.SelectedIndex = 0;
        _treePicker.BackgroundColor = Colors.White;
        _treePicker.TextColor = Color.FromArgb("#0F172A");
        _treePicker.SelectedIndexChanged += OnTreeTypeChanged;

        var header = new Label
        {
            Text = "Визуализация операций с деревьями",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#E2E8F0"),
            HorizontalTextAlignment = TextAlignment.End,
            VerticalTextAlignment = TextAlignment.Center
        };

        panel.Add(titleLabel, 0, 0);
        panel.Add(_treePicker, 1, 0);
        panel.Add(header, 2, 0);

        return panel;
    }

    private View BuildCenterPanel()
    {
        var border = new Border
        {
            Margin = new Thickness(14, 12, 14, 8),
            Stroke = Color.FromArgb("#94A3B8"),
            StrokeThickness = 1,
            BackgroundColor = Color.FromArgb("#F8FAFC"),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 12
            },
            Content = _treeCanvas
        };

        return border;
    }

    private View BuildBottomPanel()
    {
        var container = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            RowSpacing = 8,
            Padding = new Thickness(14, 0, 14, 14),
            BackgroundColor = Color.FromArgb("#CBD5E1")
        };

        var operationPanel = new OperationPanel
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(150) },
                new ColumnDefinition { Width = new GridLength(110) },
                new ColumnDefinition { Width = new GridLength(110) },
                new ColumnDefinition { Width = new GridLength(110) },
                new ColumnDefinition { Width = new GridLength(110) },
                new ColumnDefinition { Width = new GridLength(110) },
                new ColumnDefinition { Width = new GridLength(110) },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        _valueEntry.Placeholder = "Ключ";
        _valueEntry.Keyboard = Keyboard.Numeric;
        _valueEntry.BackgroundColor = Colors.White;
        _valueEntry.TextColor = Color.FromArgb("#0F172A");
        _valueEntry.ReturnType = ReturnType.Done;
        _valueEntry.Completed += (_, _) => RunOperation(OperationType.Insert);

        Button insertButton = CreateButton("Добавить", Color.FromArgb("#16A34A"));
        insertButton.Clicked += (_, _) => RunOperation(OperationType.Insert);

        Button deleteButton = CreateButton("Удалить", Color.FromArgb("#EA580C"));
        deleteButton.Clicked += (_, _) => RunOperation(OperationType.Delete);

        Button searchButton = CreateButton("Найти", Color.FromArgb("#2563EB"));
        searchButton.Clicked += (_, _) => RunOperation(OperationType.Search);

        Button clearButton = CreateButton("Очистить", Color.FromArgb("#475569"));
        clearButton.Clicked += (_, _) => ClearTree();

        _nextStepButton.Text = "Шаг";
        _nextStepButton.BackgroundColor = Color.FromArgb("#7C3AED");
        _nextStepButton.TextColor = Colors.White;
        _nextStepButton.FontAttributes = FontAttributes.Bold;
        _nextStepButton.Clicked += (_, _) => ShowNextStep();

        _autoButton.Text = "Авто";
        _autoButton.BackgroundColor = Color.FromArgb("#0F766E");
        _autoButton.TextColor = Colors.White;
        _autoButton.FontAttributes = FontAttributes.Bold;
        _autoButton.Clicked += (_, _) => ToggleAutoPlay();

        _statusLabel.TextColor = Color.FromArgb("#0F172A");
        _statusLabel.FontSize = 14;
        _statusLabel.VerticalTextAlignment = TextAlignment.Center;

        operationPanel.Add(_valueEntry, 0, 0);
        operationPanel.Add(insertButton, 1, 0);
        operationPanel.Add(deleteButton, 2, 0);
        operationPanel.Add(searchButton, 3, 0);
        operationPanel.Add(clearButton, 4, 0);
        operationPanel.Add(_nextStepButton, 5, 0);
        operationPanel.Add(_autoButton, 6, 0);
        operationPanel.Add(_statusLabel, 7, 0);

        _stepLabel.Text = "Текущий шаг: дерево пустое.";
        _stepLabel.FontSize = 14;
        _stepLabel.TextColor = Color.FromArgb("#0F172A");
        _stepLabel.BackgroundColor = Color.FromArgb("#F8FAFC");
        _stepLabel.Padding = new Thickness(12, 8);

        var stepBorder = new Border
        {
            Stroke = Color.FromArgb("#94A3B8"),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 10
            },
            Content = _stepLabel
        };

        container.Add(operationPanel, 0, 0);
        container.Add(stepBorder, 0, 1);

        return container;
    }

    private static Button CreateButton(string text, Color backgroundColor)
    {
        return new Button
        {
            Text = text,
            BackgroundColor = backgroundColor,
            TextColor = Colors.White,
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 8,
            Padding = new Thickness(10, 8)
        };
    }

    private void OnTreeTypeChanged(object? sender, EventArgs e)
    {
        if (_treePicker.SelectedItem is not TreeTypeViewModel selected)
            return;

        StopAutoPlay();
        _viewModel.SelectTree(selected);
        ResetVisualization();
    }

    private async void RunOperation(OperationType operationType)
    {
        if (!int.TryParse(_valueEntry.Text, out int key))
        {
            await _dialogService.ShowErrorAsync(this, "Введите целое число.");
            return;
        }

        StopAutoPlay();

        IReadOnlyList<TreeOperationStep> steps = _operationExecutor.Execute(_viewModel.CurrentTree, operationType, key);
        _stepPlayer.Load(steps);

        TreeOperationStep? firstStep = _stepPlayer.First();
        if (firstStep is not null)
            ShowStep(firstStep);

        _statusLabel.Text = $"Операция: {GetOperationTitle(operationType)}, ключ: {key}. Шагов: {steps.Count}.";
    }

    private void ShowNextStep()
    {
        TreeOperationStep? step = _stepPlayer.Next();
        if (step is null)
            return;

        ShowStep(step);

        if (_stepPlayer.CurrentIndex >= _stepPlayer.Count - 1)
            StopAutoPlay();
    }

    private void ToggleAutoPlay()
    {
        if (!_stepPlayer.HasSteps)
            return;

        if (_isAutoPlaying)
        {
            StopAutoPlay();
            return;
        }

        _timer ??= _animationService.CreateTimer(Dispatcher, TimeSpan.FromMilliseconds(850), ShowNextStep);
        _isAutoPlaying = true;
        _autoButton.Text = "Стоп";
        _timer.Start();
    }

    private void StopAutoPlay()
    {
        _timer?.Stop();
        _isAutoPlaying = false;
        _autoButton.Text = "Авто";
    }

    private void ClearTree()
    {
        StopAutoPlay();
        _viewModel.CurrentTree.Clear();
        _stepPlayer.Load(Array.Empty<TreeOperationStep>());
        ResetVisualization();
        _statusLabel.Text = "Дерево очищено.";
    }

    private void ResetVisualization()
    {
        _treeCanvas.SetSnapshot(_viewModel.CurrentTree.CreateSnapshot());
        _statusLabel.Text = _viewModel.Status;
        _stepLabel.Text = "Текущий шаг: дерево пустое или операция еще не выбрана.";
    }

    private void ShowStep(TreeOperationStep step)
    {
        _treeCanvas.SetSnapshot(step.Snapshot);
        _stepLabel.Text = $"Текущий шаг: {step.Description}";
    }

    private static string GetOperationTitle(OperationType operationType)
    {
        return operationType switch
        {
            OperationType.Insert => "добавление",
            OperationType.Delete => "удаление",
            OperationType.Search => "поиск",
            _ => operationType.ToString()
        };
    }
}
