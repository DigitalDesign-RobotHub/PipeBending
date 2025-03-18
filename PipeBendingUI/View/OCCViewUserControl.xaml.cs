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
	public occView occView => this.model.OccCanvas.View;

	public OCCViewUserControl( ) {
		InitializeComponent( );
		//绑定From
		this.model = new( );
		this.OCCCanvas_WindowsFormsHost.Child = this.model.OccCanvas;
		//注册点击事件
		this.model.OccCanvas.OnAISSelectionEvent += OnAISSelection;
		// 注册 SizeChanged 事件
		this.SizeChanged += ( s, e ) => {
			this.model.Width = this.ActualWidth;
			this.model.Height = this.ActualHeight;
		};
		this.DataContext = this.model;
	}

	public void OnAISSelection( AShape theAIS ) { }
}
