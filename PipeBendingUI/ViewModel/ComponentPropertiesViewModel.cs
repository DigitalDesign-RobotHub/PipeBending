using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using IMKernel.Model;
using IMKernel.Utils;
using IMKernel.ViewModel;

using OCCTK.Extension;
using OCCTK.OCC.gp;

using PipeBendingUI.Command;
using PipeBendingUI.Message;

using Component = IMKernel.Model.Component;

namespace PipeBendingUI.ViewModel;

public partial class ComponentPropertiesViewModel:ObservableObject {
    public ComponentPropertiesViewModel() {
        Name = "新建组件";
        Connection = new();
        CreateComponentState = Visibility.Visible;
        SaveComponentState = Visibility.Collapsed;
        PoseViewModel = new PoseViewModel() {
            Context = App.Current.ThreeDimensionContextManager.MainContext
        };
        MovementFormula = MovementFormula.Static; //设置默认运动类型为静止
        SelectedParentComponent = OriginComponent.Instance;
        IsAddToWorkSpace = false;
    }

    public bool IsComponentValid {
        get {
            if( Name == "" || Name == null || MovementFormula == null ) {
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

    partial void OnNameChanged( string value ) {
        CreateComponentCommand.NotifyCanExecuteChanged();
    }

    public ObservableCollection<Trsf> Connection { get; set; }

    [ObservableProperty]
    private MovementFormula movementFormula;

    partial void OnMovementFormulaChanged( MovementFormula value ) {
        CreateComponentCommand.NotifyCanExecuteChanged();
    }

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

    [ObservableProperty]
    private Component selectedParentComponent;

    partial void OnSelectedParentComponentChanged( Component value ) {
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

    [ObservableProperty]
    private bool isAddToWorkSpace;

    [RelayCommand(CanExecute = nameof(IsComponentValid))]
    private void CreateComponent() {
        Trsf tWithParent;
        if( Parent == null ) {
            tWithParent = PoseViewModel.ThePose.Datum;
        } else {
            tWithParent =
                -( Parent.Datum * Parent.Connection[ParentConnection] ) * PoseViewModel.ThePose.Datum;
        }
        App.Current.CommandManager.Execute(
            new CreateComponentCommand(),
            (
                new Component(
                    name: Name,
                    tWithParent,
                    connection: Connection,
                    movementFormula: MovementFormula,
                    initMovement: InitMovement,
                    minMovement: MinMovement,
                    maxMovement: MaxMovement,
                    shape: Shape,
                    parent: Parent,
                    parentConnection: ParentConnection
                ),
                IsAddToWorkSpace
            )
        );
    }

    public Visibility SaveComponentState { get; set; }

    [RelayCommand]
    private void SaveComponent() {
        //App.Current.CommandManager.Execute(new SaveComponentCommand(), component);
    }

    [RelayCommand]
    private void CancerComponent() {
        WeakReferenceMessenger.Default.Send(new PropertiesUIFinishedMessage());
    }
}
