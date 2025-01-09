using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DevExpress.Charts.Native;
using DevExpress.XtraRichEdit.Import.Doc;

using IMKernelUI.Command;
using IMKernel.Visualization;

using log4net;

namespace PipeBendingUI.ViewModel;

public partial class OCCViewViewModel:ObservableObject {
	private static readonly ILog log = LogManager.GetLogger(typeof(OCCViewViewModel));

	private static readonly PipeBendingUI.Singleton.CommandManager commandManager =
		App.Current.CommandManager;
	public readonly OCCCanvas OccCanvas;

	public OCCViewViewModel( ) {
		if( App.Current.ThreeDimensionContextManager.MainContext == null ) {
			OccCanvas = new OCCCanvas(
				App.Current.ThreeDimensionContextManager.CreateContext( ),
				App.Current.CommandManager
			);
		} else {
			OccCanvas = new OCCCanvas(
				App.Current.ThreeDimensionContextManager.MainContext,
				App.Current.CommandManager
			);
		}
	}

	public OCCViewViewModel( OCCCanvas canvas ) {
		// 初始化 ViewModel 或需要的资源
		OccCanvas = canvas;
	}

	private double _width;
	public double Width {
		get => _width;
		set {
			_width = value;
			CalculatePopupSize( );
		}
	}

	private double _height;
	public double Height {
		get => _height;
		set {
			_height = value;
			CalculatePopupSize( );
		}
	}

	private void CalculatePopupSize( ) {
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
	private void FrontView( ) => commandManager.Execute(new FrontViewCommand( ), OccCanvas.View);

	[RelayCommand]
	private void BackView( ) => commandManager.Execute(new BackViewCommand( ), OccCanvas.View);

	[RelayCommand]
	private void TopView( ) => commandManager.Execute(new TopViewCommand( ), OccCanvas.View);

	[RelayCommand]
	private void BottomView( ) => commandManager.Execute(new BottomViewCommand( ), OccCanvas.View);

	[RelayCommand]
	private void LeftView( ) => commandManager.Execute(new LeftViewCommand( ), OccCanvas.View);

	[RelayCommand]
	private void RightView( ) => commandManager.Execute(new RightViewCommand( ), OccCanvas.View);

	[RelayCommand]
	private void AxoView( ) => commandManager.Execute(new AxoViewCommand( ), OccCanvas.View);

	[RelayCommand]
	private void FitAll( ) => commandManager.Execute(new FitAllCommand( ), OccCanvas.View);
}
