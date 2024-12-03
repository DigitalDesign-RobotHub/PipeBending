using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IMKernel.Visualization;
using OCCTK.OCC.AIS;

namespace PipeBending.View;

/// <summary>
/// OCCViewUserControl.xaml 的交互逻辑
/// </summary>
public partial class OCCViewUserControl : UserControl, IAISSelectionHandler
{
    public OCCViewUserControl()
    {
        InitializeComponent();
        OCCCanvas theOCCCanvas;
        if (App.Current.ThreeDimensionContextManager.MainContext == null)
        {
            var context = App.Current.ThreeDimensionContextManager.CreateContext();
            theOCCCanvas = new OCCCanvas(context);
        }
        else
        {
            theOCCCanvas = new OCCCanvas(App.Current.ThreeDimensionContextManager.MainContext);
        }
        //注册鼠标事件
        theOCCCanvas.OnAISSelectionEvent += OnAISSelection;
        OCCCanvas_WindowsFormsHost.Child = theOCCCanvas;
    }

    public void OnAISSelection(AShape theAIS) { }
}
