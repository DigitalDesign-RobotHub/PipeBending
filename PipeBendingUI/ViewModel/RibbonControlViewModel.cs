using System;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using IMKernel.OCCExtension.Serialization;
using IMKernel.Visualization;

using IMKernelUI.Command;
using IMKernelUI.Message;

using log4net;

using Newtonsoft.Json;

using OCCTK.OCC.gp;

using PipeBendingUI.Command;
namespace PipeBendingUI.ViewModel;

public partial class RibbonControlViewModel:ObservableObject {
	private static readonly ILog log = LogManager.GetLogger(typeof(RibbonControlViewModel));

	private static readonly IMKernelUI.Singleton.CommandManager commandManager = App.Current.CommandManager;
	private static ThreeDimensionContext? threeDimensionContext => App.Current.ThreeDimensionContextManager.MainContext;

	private OCCCanvas mainCanvas;

	public RibbonControlViewModel( ) {

		VisibilityControl( ); //权限控制
		this.CreateNewComponentCommand = new CreateNewComponent( ); //创建新部件
		this.CreateNewRobotCommand = new CreateNewComponent( ); //todo 创建新机器人

		WeakReferenceMessenger.Default.Register<CanvasCreatedMessage>(this, ( r, m ) => {
			if( m.Value.contextID != 0 ) {
				return;
			}
			//创建画布
			this.mainCanvas = threeDimensionContext?.CreateView(commandManager) ?? throw new Exception("创建画布失败");
			this.IsShowOriginTrihedron = m.Value.orignTri;
			this.IsShowViewTrihedron = m.Value.viewTri;
			this.IsShowViewCube = m.Value.viewCube;
			this.IsShowGraduatedTrihedron = false;
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
			this.mainCanvas.View.DisplayDefault_GraduatedTrihedron( );
		} else {
			this.mainCanvas.View.Hide_GraduatedTrihedron( );
		}
		this.mainCanvas.Update( );
	}
	#endregion

	#endregion

	#region Command
#if DEBUG
	[RelayCommand]
	private void Test01( ) {
		//Trsf t=new(new Vec(4, 3, 9),new Quat(90.0.ToRadians(),45.0.ToRadians(),15.0.ToRadians(),EulerSequence.Extrinsic_XYZ));

		//待序列化的对象
		Trsf t=new(new Vec(4, 3, 9),new Quat(1,2,3,1));

		//+ 序列化
		log.Debug("序列化Trsf");
		string json = JsonConvert.SerializeObject(t, Formatting.Indented, new TrsfJsonConverter());
		log.Debug($"Trsf: {t}");
		log.Debug("Json: {json}");

		//+ 反序列化
		Trsf t2 = JsonConvert.DeserializeObject<Trsf>(json, new TrsfJsonConverter());
		log.Debug($"反序列化Trsf: {t2}");
	}
#endif
	[RelayCommand]
	private void Undo( ) => App.Current.CommandManager.Undo( );

	public ICommand CreateNewComponentCommand { get; }
	public ICommand CreateNewRobotCommand { get; }

	[RelayCommand]
	private void FrontView( ) => commandManager.Execute(new FrontViewCommand( ), this.mainCanvas.View);

	[RelayCommand]
	private void BackView( ) => commandManager.Execute(new BackViewCommand( ), this.mainCanvas.View);

	[RelayCommand]
	private void TopView( ) => commandManager.Execute(new TopViewCommand( ), this.mainCanvas.View);

	[RelayCommand]
	private void BottomView( ) => commandManager.Execute(new BottomViewCommand( ), this.mainCanvas.View);

	[RelayCommand]
	private void LeftView( ) => commandManager.Execute(new LeftViewCommand( ), this.mainCanvas.View);

	[RelayCommand]
	private void RightView( ) => commandManager.Execute(new RightViewCommand( ), this.mainCanvas.View);

	[RelayCommand]
	private void AxoView( ) => commandManager.Execute(new AxoViewCommand( ), this.mainCanvas.View);

	[RelayCommand]
	private void FitAll( ) => commandManager.Execute(new FitAllCommand( ), this.mainCanvas.View);

	#endregion

}
