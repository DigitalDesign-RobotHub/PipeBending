using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using IMKernel.Command;
using IMKernel.Visualization;

using log4net;

using PipeBendingUI.Command;

namespace PipeBendingUI.ViewModel;

public partial class RibbonControlViewModel:ObservableObject {
    private static readonly ILog log = LogManager.GetLogger(typeof(RibbonControlViewModel));

    private static readonly Singleton.CommandManager commandManager = App.Current.CommandManager;
    private static ThreeDimensionContext? threeDimensionContext => App.Current.ThreeDimensionContextManager.MainContext;

    public RibbonControlViewModel() {
    VisibilityControl(); //权限控制
    CreateNewComponentCommand = new CreateNewComponent(); //创建新部件
    CreateNewRobotCommand = new CreateNewComponent(); //todo 创建新机器人
    IsShowOriginTrihedron = false;
    IsShowViewTrihedron = false;
    IsShowViewCube = true;
    IsShowGraduatedTrihedron = false;
    }

    [ObservableProperty]
    private bool isShowOriginTrihedron;
    partial void OnIsShowOriginTrihedronChanged(bool value) {
    threeDimensionContext?.DisplayOriginTrihedron(value);
    }

    [ObservableProperty]
    private bool isShowViewTrihedron;
    partial void OnIsShowViewTrihedronChanged(bool value) {
    threeDimensionContext?.DisplayViewTrihedron(value);
    }

    [ObservableProperty]
    private bool isShowViewCube;
    partial void OnIsShowViewCubeChanged(bool value) {
    threeDimensionContext?.DisplayViewCube(value);
    }

    [ObservableProperty]
    private bool isShowGraduatedTrihedron;
    partial void OnIsShowGraduatedTrihedronChanged(bool value) {
    threeDimensionContext?.DisplayGraduatedTrihedron(value);
    }

    [RelayCommand]
    private void Undo() => App.Current.CommandManager.Undo();

    public ICommand CreateNewComponentCommand { get; }
    public ICommand CreateNewRobotCommand { get; }

    [RelayCommand]
    private void FrontView() =>
        commandManager.Execute(
            new FrontViewCommand(),
            App.Current.ThreeDimensionContextManager.Contexts[0].ViewList[0].View
        );

    [RelayCommand]
    private void BackView() =>
        commandManager.Execute(
            new BackViewCommand(),
            App.Current.ThreeDimensionContextManager.Contexts[0].ViewList[0].View
        );

    [RelayCommand]
    private void TopView() =>
        commandManager.Execute(
            new TopViewCommand(),
            App.Current.ThreeDimensionContextManager.Contexts[0].ViewList[0].View
        );

    [RelayCommand]
    private void BottomView() =>
        commandManager.Execute(
            new BottomViewCommand(),
            App.Current.ThreeDimensionContextManager.Contexts[0].ViewList[0].View
        );

    [RelayCommand]
    private void LeftView() =>
        commandManager.Execute(
            new LeftViewCommand(),
            App.Current.ThreeDimensionContextManager.Contexts[0].ViewList[0].View
        );

    [RelayCommand]
    private void RightView() =>
        commandManager.Execute(
            new RightViewCommand(),
            App.Current.ThreeDimensionContextManager.Contexts[0].ViewList[0].View
        );

    [RelayCommand]
    private void AxoView() =>
        commandManager.Execute(
            new AxoViewCommand(),
            App.Current.ThreeDimensionContextManager.Contexts[0].ViewList[0].View
        );

    [RelayCommand]
    private void FitAll() =>
        commandManager.Execute(
            new FitAllCommand(),
            App.Current.ThreeDimensionContextManager.Contexts[0].ViewList[0].View
        );
}
