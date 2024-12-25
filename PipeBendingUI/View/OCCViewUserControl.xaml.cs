using System.Windows.Controls;

using IMKernel.Visualization;

using OCCTK.OCC.AIS;

using PipeBendingUI.ViewModel;

using occView = OCCTK.OCC.V3d.View;

namespace PipeBendingUI.View;

/// <summary>
/// OCCViewUserControl.xaml 的交互逻辑
/// </summary>
public partial class OCCViewUserControl:UserControl, IAISSelectionHandler {
	public readonly OCCViewViewModel model;
	public occView occView => model.OccCanvas.View;

	public OCCViewUserControl( ) {
		InitializeComponent( );

		model = new( );
		OCCCanvas_WindowsFormsHost.Child = model.OccCanvas;
		//注册鼠标事件
		model.OccCanvas.OnAISSelectionEvent += OnAISSelection;
		// 注册 SizeChanged 事件
		this.SizeChanged += ( s, e ) => {
			model.Width = this.ActualWidth;
			model.Height = this.ActualHeight;
		};
		DataContext = model;
	}

	public void OnAISSelection( AShape theAIS ) { }
}
