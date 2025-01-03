using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using IMKernel.Model;
using IMKernel.OCCExtension;
using IMKernel.Visualization;

using IMKernelUI.ViewModel;

using OCCTK.Extension;
using OCCTK.OCC.gp;

using PipeBendingUI.Command;
using PipeBendingUI.Message;
using IMKernelUI.Message;

using Component = IMKernel.Model.Component;
using IMKernelUI.Interfaces;
using IMKernel.Kinematic;

namespace PipeBendingUI.ViewModel;

public partial class ComponentViewModel:ObservableObject, IOCCFinilize {
	public ComponentViewModel( ) {
		//value
		Name = "新部件";
		CreateComponentState = Visibility.Visible;
		SaveComponentState = Visibility.Collapsed;
		IsAddToWorkSpace = false;
		ComponentVM = new( );
	}

	public ComponentInstance TheComponent {
		get {
			return new(Name, ComponentVM.TheComponent);
		}
		set {
			if( value != null ) {
				ComponentVM.TheComponent = value.Component;
				Name = value.Name;
			}
		}
	}

	public bool IsComponentValid {
		get {
			if( ComponentVM.Name == "" || ComponentVM.Name == null ) {
				return false;
			}
			return true;
		}
	}

	#region OCC

	//private OCCCanvas? canvas;

	#endregion

	#region Value

	[ObservableProperty]
	private string name;

	[ObservableProperty]
	private ComponentPropertiesViewModel componentVM;

	/// <summary>
	/// 可选为父部件的部件集合
	/// </summary>
	public ObservableCollection<Component> Components { protected get; set; }

	#endregion

	public Visibility CreateComponentState { get; set; }

	[ObservableProperty]
	private bool isAddToWorkSpace;

	[RelayCommand(CanExecute = nameof(IsComponentValid))]
	private void CreateComponent( ) {
		App.Current.CommandManager.Execute(new CreateComponentCommand( ), (ComponentVM.TheComponent, IsAddToWorkSpace));
	}

	public Visibility SaveComponentState { get; set; }

	[RelayCommand]
	private void SaveComponent( ) {
		//App.Current.CommandManager.Execute(new SaveComponentCommand(), component);
		ComponentVM.OCCFinilize( );
	}

	[RelayCommand]
	private void CancelComponent( ) {
		WeakReferenceMessenger.Default.Send(new PropertiesUIFinishedMessage( ));
		ComponentVM.OCCFinilize( );
	}

	#region OCC

	public void OCCFinilize( ) {
		ComponentVM.OCCFinilize( );
	}

	#endregion

}
