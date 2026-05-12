using TreeVisualizer.App.Controls;
using TreeVisualizer.App.Converters;
using TreeVisualizer.App.ViewModels;

namespace TreeVisualizer.App.Views;

public class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        BindingContext = vm;

        var canvas = new TreeCanvasControl();
        canvas.SetBinding(TreeCanvasControl.NodesProperty, nameof(MainViewModel.VisualNodes));

        var entry = new Entry { Placeholder = "Ключ", Keyboard = Keyboard.Numeric, WidthRequest = 100 };
        entry.SetBinding(Entry.TextProperty, nameof(MainViewModel.InputValue), BindingMode.TwoWay, new IntToStringConverter());

        var btnInsert = new Button { Text = "Вставить", Command = vm.InsertNodeCommand };
        var btnDelete = new Button { Text = "Удалить", Command = vm.DeleteNodeCommand };
        var btnClear = new Button { Text = "Очистить", Command = vm.ClearTreeCommand };

        var controls = new HorizontalStackLayout
        {
            Spacing = 10, 
            HorizontalOptions = LayoutOptions.Center, 
            Children = { entry, btnInsert, btnDelete, btnClear }
        };

        var status = new Label { HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0, 15, 0, 0) };
        status.SetBinding(Label.TextProperty, nameof(MainViewModel.StatusMessage));

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 20,
                Children = { canvas, controls, status }
            }
        };
    }
}