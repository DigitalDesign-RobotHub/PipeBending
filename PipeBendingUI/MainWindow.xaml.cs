using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon;
using IMKernel.Model;
using PipeBendingUI.Message;
using PipeBendingUI.View;
using PipeBendingUI.ViewModel;

namespace PipeBendingUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : ThemedWindow
{
    public MainWindow()
    {
        InitializeComponent();
        #region Message
        //创建Component窗口
        WeakReferenceMessenger.Default.Register<ComponentChangedMessage>(
            this,
            (r, m) =>
            {
                CreateComponentUI(m.Value);
            }
        );
        //删除Properties窗口
        WeakReferenceMessenger.Default.Register<PropertiesUIFinishedMessage>(
            this,
            (r, m) =>
            {
                MainWindow_Properties_Grid.Children.Clear();
                MainWindow_Properties_Grid.Children.Add(
                    new System.Windows.Controls.Label()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Content = "属性栏",
                        FontSize = 40
                    }
                );
            }
        );
        #endregion
        var ribbonControl = this.FindName("RibbonControl") as RibbonControl;
    }

    private void CreateComponentUI(Component component)
    {
        if (MainWindow_Properties_Grid.Children.Count != 0)
        {
            if (MainWindow_Properties_Grid.Children[0] is ComponentProperties)
            {
                return;
            }
        }
        MainWindow_Properties_Grid.Children.Clear();
        var componentUI = new ComponentProperties();
        var viewModel = new ComponentPropertiesViewModel();
        viewModel.Component = component;
        componentUI.DataContext = viewModel;
        MainWindow_Properties_Grid.Children.Add(componentUI);
    }
}
