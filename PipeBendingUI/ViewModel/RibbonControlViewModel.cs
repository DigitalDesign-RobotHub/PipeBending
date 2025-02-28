using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using IMKernelUI.Command;
using IMKernel.Visualization;

using IMKernelUI.Message;

using log4net;

using System;
using PipeBendingUI.Command;
using PipeBendingUI.Message;
using OCCTK.OCC.gp;
using IMKernel.OCCExtension.Serialization;
using IMKernel.OCCExtension;
using System.Diagnostics;
using Newtonsoft.Json;
namespace PipeBendingUI.ViewModel;

public partial class RibbonControlViewModel:ObservableObject {
	private static readonly ILog log = LogManager.GetLogger(typeof(RibbonControlViewModel));

	private static readonly Singleton.CommandManager commandManager = App.Current.CommandManager;
	private static ThreeDimensionContext? threeDimensionContext => App.Current.ThreeDimensionContextManager.MainContext;

	private OCCCanvas mainCanvas;

	public RibbonControlViewModel( ) {

		VisibilityControl( ); //权限控制
		CreateNewComponentCommand = new CreateNewComponent( ); //创建新部件
		CreateNewRobotCommand = new CreateNewComponent( ); //todo 创建新机器人

		WeakReferenceMessenger.Default.Register<CanvasCreatedMessage>(this, ( r, m ) => {
			if( m.Value.contextID != 0 ) {
				return;
			}
			//创建画布
			mainCanvas = threeDimensionContext?.CreateView(commandManager) ?? throw new Exception("创建画布失败");
			IsShowOriginTrihedron = m.Value.orignTri;
			IsShowViewTrihedron = m.Value.viewTri;
			IsShowViewCube = m.Value.viewCube;
			IsShowGraduatedTrihedron = false;
		});

		WeakReferenceMessenger.Default.Register<RibbonControlViewModel, MainCanvasRequestMessage>(this, ( r, m ) => {
			m.Reply(r.mainCanvas);
		}
		);

	}
	#region 画布控制


	[ObservableProperty]
	private bool isShowOriginTrihedron;
	partial void OnIsShowOriginTrihedronChanged( bool value ) {
		threeDimensionContext?.DisplayOriginTrihedron(value);
		WeakReferenceMessenger.Default.Send(new ViewStatusChangedMessage((contextID: 0, viewCube: IsShowViewCube, orignTri: IsShowOriginTrihedron, viewTri: IsShowViewTrihedron)));
	}

	[ObservableProperty]
	private bool isShowViewTrihedron;
	partial void OnIsShowViewTrihedronChanged( bool value ) {
		threeDimensionContext?.DisplayViewTrihedron(value);
		WeakReferenceMessenger.Default.Send(new ViewStatusChangedMessage((contextID: 0, viewCube: IsShowViewCube, orignTri: IsShowOriginTrihedron, viewTri: IsShowViewTrihedron)));
	}

	[ObservableProperty]
	private bool isShowViewCube;
	partial void OnIsShowViewCubeChanged( bool value ) {
		threeDimensionContext?.DisplayViewCube(value);
		WeakReferenceMessenger.Default.Send(new ViewStatusChangedMessage((contextID: 0, viewCube: IsShowViewCube, orignTri: IsShowOriginTrihedron, viewTri: IsShowViewTrihedron)));
	}

	[ObservableProperty]
	private bool isShowGraduatedTrihedron;
	partial void OnIsShowGraduatedTrihedronChanged( bool value ) {
		DisplayGraduatedTrihedron(value);
	}

	#region 刻度坐标系
	public void DisplayGraduatedTrihedron( bool showGraduatedTrihedron ) {
		if( showGraduatedTrihedron ) {
			mainCanvas.View.DisplayDefault_GraduatedTrihedron( );
		} else {
			mainCanvas.View.Hide_GraduatedTrihedron( );
		}
		mainCanvas.Update( );
	}
	#endregion

	#endregion

	#region Command
	[RelayCommand]
	private void Test01( ) {
		Trsf t=new();
		t.SetTranslationPart(new(4, 3, 9));
		var q=new Quat(90.0.ToRadians(),45.0.ToRadians(),15.0.ToRadians(),EulerSequence.Extrinsic_XYZ);
		t.SetRotationPart(q);

		log.Debug("序列化Trsf");
		log.Debug(t);
		log.Debug("Json");
		string json = JsonConvert.SerializeObject(t, Formatting.Indented, new TrsfJsonConverter());
		log.Debug(json);

		Trsf t2 = JsonConvert.DeserializeObject<Trsf>(json, new TrsfJsonConverter())?? throw new Exception("转换失败");
		log.Debug("反序列化Trsf");
		log.Debug(t2);
	}

	[RelayCommand]
	private void Undo( ) => App.Current.CommandManager.Undo( );

	public ICommand CreateNewComponentCommand { get; }
	public ICommand CreateNewRobotCommand { get; }

	[RelayCommand]
	private void FrontView( ) => commandManager.Execute(new FrontViewCommand( ), mainCanvas.View);

	[RelayCommand]
	private void BackView( ) => commandManager.Execute(new BackViewCommand( ), mainCanvas.View);

	[RelayCommand]
	private void TopView( ) => commandManager.Execute(new TopViewCommand( ), mainCanvas.View);

	[RelayCommand]
	private void BottomView( ) => commandManager.Execute(new BottomViewCommand( ), mainCanvas.View);

	[RelayCommand]
	private void LeftView( ) => commandManager.Execute(new LeftViewCommand( ), mainCanvas.View);

	[RelayCommand]
	private void RightView( ) => commandManager.Execute(new RightViewCommand( ), mainCanvas.View);

	[RelayCommand]
	private void AxoView( ) => commandManager.Execute(new AxoViewCommand( ), mainCanvas.View);

	[RelayCommand]
	private void FitAll( ) => commandManager.Execute(new FitAllCommand( ), mainCanvas.View);

	#endregion

}
