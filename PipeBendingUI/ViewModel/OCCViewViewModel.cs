using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Charts.Native;
using IMKernel.Command;
using IMKernel.Visualization;
using log4net;

namespace PipeBendingUI.ViewModel;

[ObservableObject]
public partial class OCCViewViewModel
{
    private static readonly ILog log = LogManager.GetLogger(typeof(OCCViewViewModel));

    private static readonly PipeBendingUI.Singleton.CommandManager commandManager =
        App.Current.CommandManager;
    public readonly OCCCanvas occCanvas;

    public OCCViewViewModel() { }

    public OCCViewViewModel(OCCCanvas canvas)
    {
        // 初始化 ViewModel 或需要的资源
        occCanvas = canvas;
    }

    private double _width;
    public double Width
    {
        get => _width;
        set
        {
            _width = value;
            CalculatePopupSize();
            //Debug.WriteLine(
            //    $"W:{PopupWidth} H:{PopupHeight} HO:{PopupHorizontalOffset} VO:{PopupVerticalOffset}"
            //);
        }
    }

    private double _height;
    public double Height
    {
        get => _height;
        set
        {
            _height = value;
            CalculatePopupSize();
            //Debug.WriteLine(
            //    $"W:{PopupWidth} H:{PopupHeight} HO:{PopupHorizontalOffset} VO:{PopupVerticalOffset}"
            //);
        }
    }

    private void CalculatePopupSize()
    {
        PopupWidth = Width * 0.8;
        PopupHeight = 80;
        PopupHorizontalOffset = Width * 0;
        PopupVerticalOffset = -Height * 0.45;
    }

    [ObservableProperty]
    private double popupWidth;

    [ObservableProperty]
    private double popupHeight;

    [ObservableProperty]
    private double popupHorizontalOffset;

    [ObservableProperty]
    private double popupVerticalOffset;

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
}
