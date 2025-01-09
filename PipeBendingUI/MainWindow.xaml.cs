using CommunityToolkit.Mvvm.Messaging;

using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon;

using IMKernel.Model;

using IMKernelUI.View;
using IMKernelUI.ViewModel;

using PipeBendingUI.Message;
using PipeBendingUI.View;
using PipeBendingUI.ViewModel;

namespace PipeBendingUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow:ThemedWindow {
	public MainWindow( ) {
		InitializeComponent( );

		#region Message

		// 修改原有部件
		WeakReferenceMessenger.Default.Register<ComponentChangingMessage>(
			this,
			( r, m ) => {
				CreateComponentUI(m.OldComponent);
			}
		);

		//删除Properties窗口
		WeakReferenceMessenger.Default.Register<PropertiesUIFinishedMessage>(
			this,
			( r, m ) => {
				MainWindow_Properties_Grid.Children.Clear( );
				MainWindow_Properties_Grid.Children.Add(
					new System.Windows.Controls.Label( ) {
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

	/// <summary>
	/// 创建或保存新部件
	/// </summary>
	/// <param name="component"></param>
	private void CreateComponentUI( ComponentInstance? component = null ) {
		if( MainWindow_Properties_Grid.Children.Count != 0 && MainWindow_Properties_Grid.Children[0] is ComponentViewModel ) {
			return;
		}

		MainWindow_Properties_Grid.Children.Clear( );
		var componentUI = new ComponentView(){ DataContext=new ComponentViewModel()};

		if( component != null ) {
			var a=(ComponentViewModel)componentUI.DataContext;
			a.TheComponentInstance = component ?? throw new System.Exception("传入了空部件");
		}
		MainWindow_Properties_Grid.Children.Add(componentUI);
	}
}
