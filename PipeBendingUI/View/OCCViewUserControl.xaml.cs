using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using IMKernel.Visualization;
using OCCTK.OCC.AIS;
using PipeBendingUI.ViewModel;
using occView = OCCTK.OCC.V3d.View;

namespace PipeBendingUI.View;

/// <summary>
/// OCCViewUserControl.xaml 的交互逻辑
/// </summary>
public partial class OCCViewUserControl : UserControl, IAISSelectionHandler
{
    public readonly OCCViewViewModel _Model;
    public occView occView => _Model.occCanvas.View;

    public OCCViewUserControl()
    {
        InitializeComponent();
        OCCCanvas theOCCCanvas;
        if (App.Current.ThreeDimensionContextManager.MainContext == null)
        {
            theOCCCanvas = new OCCCanvas(
                App.Current.ThreeDimensionContextManager.CreateContext(),
                App.Current.CommandManager
            );
        }
        else
        {
            theOCCCanvas = new OCCCanvas(
                App.Current.ThreeDimensionContextManager.MainContext,
                App.Current.CommandManager
            );
        }
        _Model = new(theOCCCanvas);
        OCCCanvas_WindowsFormsHost.Child = theOCCCanvas;
        //注册鼠标事件
        theOCCCanvas.OnAISSelectionEvent += OnAISSelection;
        // 注册 SizeChanged 事件
        this.SizeChanged += (s, e) =>
        {
            _Model.Width = this.ActualWidth;
            _Model.Height = this.ActualHeight;
        };
        DataContext = _Model;
    }

    public void OnAISSelection(AShape theAIS) { }
}
