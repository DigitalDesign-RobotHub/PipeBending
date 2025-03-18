using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using IMKernel.Visualization;

using IMKernelUI.Command;
using IMKernelUI.Singleton;

using log4net;

namespace PipeBendingUI.ViewModel;

public partial class OCCViewViewModel:ObservableObject {
	private static readonly ILog log = LogManager.GetLogger(typeof(OCCViewViewModel));

	private static readonly CommandManager commandManager =
		App.Current.CommandManager;
	public readonly OCCCanvas OccCanvas;

	public OCCViewViewModel( ) {
		this.OccCanvas = new OCCCanvas(
			App.Current.ThreeDimensionContextManager.MainContext ?? App.Current.ThreeDimensionContextManager.CreateContext( ),
			App.Current.CommandManager
		);
	}

	public OCCViewViewModel( OCCCanvas canvas ) {
		// 初始化 ViewModel 或需要的资源
		this.OccCanvas = canvas;
	}

	private double _width;
	public double Width {
		get => this._width;
		set {
			this._width = value;
			CalculatePopupSize( );
		}
	}

	private double _height;
	public double Height {
		get => this._height;
		set {
			this._height = value;
			CalculatePopupSize( );
		}
	}

	private void CalculatePopupSize( ) {
		this.PopupWidth = this.Width * 0.8;
		this.PopupHeight = 80;
		this.PopupHorizontalOffset = this.Width * 0;
		this.PopupVerticalOffset = -this.Height * 0.45;
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
	private void FrontView( ) => commandManager.Execute(new FrontViewCommand( ), this.OccCanvas.View);

	[RelayCommand]
	private void BackView( ) => commandManager.Execute(new BackViewCommand( ), this.OccCanvas.View);

	[RelayCommand]
	private void TopView( ) => commandManager.Execute(new TopViewCommand( ), this.OccCanvas.View);

	[RelayCommand]
	private void BottomView( ) => commandManager.Execute(new BottomViewCommand( ), this.OccCanvas.View);

	[RelayCommand]
	private void LeftView( ) => commandManager.Execute(new LeftViewCommand( ), this.OccCanvas.View);

	[RelayCommand]
	private void RightView( ) => commandManager.Execute(new RightViewCommand( ), this.OccCanvas.View);

	[RelayCommand]
	private void AxoView( ) => commandManager.Execute(new AxoViewCommand( ), this.OccCanvas.View);

	[RelayCommand]
	private void FitAll( ) => commandManager.Execute(new FitAllCommand( ), this.OccCanvas.View);
}
