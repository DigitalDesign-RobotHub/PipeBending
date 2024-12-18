using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using IMKernel.Model;
using IMKernel.OCCExtension;
using IMKernel.Utils;
using IMKernel.ViewModel;
using OCCTK.Extension;
using OCCTK.OCC.gp;
using PipeBendingUI.Command;
using PipeBendingUI.Message;
using Component = IMKernel.Model.Component;

namespace PipeBendingUI.ViewModel;

[ObservableObject]
public partial class ComponentPropertiesViewModel
{
    public ComponentPropertiesViewModel()
    {
        CreateComponentState = Visibility.Visible;
        SaveComponentState = Visibility.Collapsed;
        PoseViewModel = new PoseViewModel();
        Component = new Component();
    }

    public bool IsComponentValid
    {
        get
        {
            var c = Component;
            if (c.Name == "" || c.TransfromWithParent == null || c.MovementFormula == null)
            {
                return false;
            }
            return true;
        }
    }

    #region 属性和字段
    [ObservableProperty]
    private PoseViewModel poseViewModel;

    [ObservableProperty]
    private string name;
    public long ID { get; protected set; }

    public ObservableCollection<Trsf> Connection { get; set; }

    [ObservableProperty]
    private MovementFormula movementFormula;

    [ObservableProperty]
    private double initMovement;

    [ObservableProperty]
    private double minMovement;

    [ObservableProperty]
    private double maxMovement;

    [ObservableProperty]
    private XShape? shape;

    [ObservableProperty]
    private Component? parent;

    [ObservableProperty]
    private int parentConnection;
    #endregion
    //partial void OnComponentChanged(Component value)
    //{
    //    PoseViewModel.ThePose = new(Component.Name, value.Datum);
    //    // 当Component属性发生变化时，这个方法会被自动调用
    //    //if (value != null)
    //    //{
    //    //    // 监听Component的Name属性变化
    //    //    value.PropertyChanged += Component_PropertyChanged;
    //    //}
    //    CreateComponentCommand.NotifyCanExecuteChanged();
    //    Debug.WriteLine("Component Changed");
    //}

    //partial void OnComponentChanged(Component? oldValue, Component newValue)
    //{
    //    Debug.WriteLine($"old:{oldValue?.Name} | new:{newValue.Name}");
    //}

    //private void Component_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(Component.Name))
    //    {
    //        // 当Name属性发生变化时，执行你想要的操作
    //        OnPropertyChanged(nameof(Component));
    //        // 通知命令状态可能已改变
    //        CreateComponentCommand.NotifyCanExecuteChanged();
    //        // 或者执行其他特定的逻辑
    //    }
    //}

    [ObservableProperty]
    private MovementFormula movement;

    [ObservableProperty]
    private Component selectedParentComponent;

    partial void OnSelectedParentComponentChanged(Component value)
    {
        Parent = value;
    }

    public ObservableCollection<Component> Components =>
        App.Current.StaticResourceManager.Components;

    public List<MovementFormula> DefaultMovements { get; } =
        new()
        {
            MovementFormula.Static,
            MovementFormula.dX_Plus,
            MovementFormula.dX_Minus,
            MovementFormula.dY_Plus,
            MovementFormula.dY_Minus,
            MovementFormula.dZ_Plus,
            MovementFormula.dZ_Minus,
            MovementFormula.rX_Plus,
            MovementFormula.rX_Minus,
            MovementFormula.rY_Plus,
            MovementFormula.rY_Minus,
            MovementFormula.rZ_Plus,
            MovementFormula.rZ_Minus,
        };

    public Visibility CreateComponentState { get; set; }
    private bool IsAddToWorkSpace;

    [RelayCommand(CanExecute = nameof(IsComponentValid))]
    private void CreateComponent()
    {
        App.Current.CommandManager.Execute(
            new CreateComponentCommand(),
            (component, IsAddToWorkSpace)
        );
    }

    public Visibility SaveComponentState { get; set; }

    [RelayCommand]
    private void SaveComponent()
    {
        App.Current.CommandManager.Execute(new SaveComponentCommand(), component);
    }

    [RelayCommand]
    private void CancerComponent()
    {
        WeakReferenceMessenger.Default.Send(new PropertiesUIFinishedMessage());
    }
}
