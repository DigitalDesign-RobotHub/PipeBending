using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using IMKernel.Model;

using IMKernelUI.Interfaces;
using IMKernelUI.ViewModel;

using log4net;

using PipeBendingUI.Command;
using PipeBendingUI.Message;

using Component = IMKernel.Model.Component;

namespace PipeBendingUI.ViewModel;

public partial class ComponentViewModel:ObservableObject, IOCCFinilize {
	private static readonly ILog log = LogManager.GetLogger(typeof(ComponentViewModel));
	public ComponentViewModel( ) {
		//value
		this.Name = "新部件实例";
		this.CreateComponentState = Visibility.Visible;
		this.ChangingComponentState = Visibility.Collapsed;
		this.Components = new( );
		this.IsAddToWorkSpace = false;
		this.ComponentVM = new( );
	}

	#region OCC

	//private OCCCanvas? canvas;

	public void OCCFinilize( ) {
		this.ComponentVM.OCCFinilize( );
	}

	#endregion

	#region Value

	/// <summary>
	/// VM对应的部件实例对象
	/// </summary>
	public ComponentInstance TheComponentInstance {
		get {
			return new(this.Name, this.ComponentVM.TheComponent);
		}
		set {
			if( value != null ) {
				this.ComponentVM.TheComponent = value.Component;
				this.Name = value.Name;
			}
		}
	}

	[ObservableProperty]
	private string name;

	[ObservableProperty]
	private ComponentPropertiesViewModel componentVM;

	/// <summary>
	/// 可选为父部件的部件集合
	/// </summary>
	public ObservableCollection<Component> Components { protected get; set; }

	#endregion

	#region View

	/// <summary>
	/// 是否满足创建部件的条件
	/// </summary>
	public bool IsComponentValid {
		get {
			if( this.ComponentVM.Name == "" || this.ComponentVM.Name == null ) {
				return false;
			}
			return true;
		}
	}

	//同一个View同时用于创建新部件实例和修改既有部件实例

	/// <summary>
	/// 是否在创建的同时加入工作空间树
	/// </summary>
	[ObservableProperty]
	private bool isAddToWorkSpace;

	/// <summary>
	/// 创建新部件
	/// </summary>
	public Visibility CreateComponentState { get; set; }


	[RelayCommand(CanExecute = nameof(IsComponentValid))]
	private void CreateComponent( ) {
		App.Current.CommandManager.Execute(new CreateComponentCommand( ), (this.ComponentVM.TheComponent, this.IsAddToWorkSpace));
	}

	/// <summary>
	/// 修改部件
	/// </summary>
	public Visibility ChangingComponentState { get; set; }

	[RelayCommand]
	private void SaveComponent( ) {
		//App.Current.CommandManager.Execute(new SaveComponentCommand(), component);
		this.ComponentVM.OCCFinilize( );
	}

	[RelayCommand]
	private void CancelComponent( ) {
		WeakReferenceMessenger.Default.Send(new PropertiesUIFinishedMessage( ));
		this.ComponentVM.OCCFinilize( );
	}

	#endregion

}
