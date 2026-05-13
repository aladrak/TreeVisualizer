TreeVisualizer/
├── TreeVisualizer.Domain/                   # Бизнес-логика
│   ├── Interfaces/
│   │   ├── ITree.cs
│   │   ├── ITreeNode.cs
│   │   └── ITreeOperations.cs
│   ├── Nodes/
│   │   ├── TreeNodeBase.cs
│   │   └── BinaryTreeNode.cs
│   ├── Trees/
│   │   ├── TreeBase.cs
│   │   └── BinarySearchTree.cs
│   └── Enums/
│       ├── TreeType.cs
│       └── NodeState.cs      # Normal, Visited, Target, Highlight, Error
├── TreeVisualizer.Infrastructure/           # Сервисы, алгоритмы, DTO, фабрики
│   ├── Layout/
│   │   ├── ITreeLayoutService.cs
│   │   └── BinaryTreeLayoutService.cs
│   ├── Factories/
│   │   └── TreeFactory.cs
│   ├── Dto/
│   │   └── VisualNode.cs    # Узел + координаты + состояние + ссылка на Domain-узел
│   └── Interfaces/
│       └── ILayoutSettings.cs
├── TreeVisualizer.App/             # UI + MVVM
│   ├── Resources/
│   ├── Platforms/      # Стандартная реализация только под windows (можно расширить)
│   ├── ViewModels/
│   │   ├── Interfaces/
│   │   │   └── IMainViewModel.cs
│   │   ├── MainViewModel.cs
│   │   └── TreeVisualizationViewModel.cs
│   ├── Views/
│   │   └── MainPage.xaml/.cs
│   ├── Controls/
│   │   └── TreeCanvasControl.xaml/.cs
│   └── Converters/
│       └── NodeStateToColorConverter.cs
├── MauiProgram.cs            # DI, регистрация сервисов, настройка приложения
└── App.xaml
