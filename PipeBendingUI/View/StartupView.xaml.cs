using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using PipeBendingUI.ViewModel;

namespace PipeBendingUI.View;

public class StartupResult
{
    public bool MainWindow = false;
    public bool Todo = false;
    public bool Closed = false;
}

/// <summary>
/// Interaction logic for StartupView.xaml
/// </summary>
public partial class StartupView : ThemedWindow
{
    public StartupView()
    {
        Model = new();
        DataContext = Model;
        InitializeComponent();
        //异步延迟加载
        Dispatcher.InvokeAsync(() => Model.DeferredInit());
    }

    public StartupResult Result { get; private set; }

    public StartupViewModel Model;
}
