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
		canvas = WeakReferenceMessenger.Default.Send<MainCanvasRequestMessage>( );
		CreateComponentState = Visibility.Visible;
		SaveComponentState = Visibility.Collapsed;
		IsAddToWorkSpace = false;
		DefaultMovements = new( )
		{
			MovementFormula.Static,
			MovementFormula.dX_Plus,
			MovementFormula.dX_Minus,
			MovementFormula.dY_Plus,
			MovementFormula.dY_Minus,
			MovementFormula.dZ_Plus,
			MovementFormula.dZ_Minus,
			MovementFormula.rX_Plus,
			MovementFormula.rX_Minus,
			MovementFormula.rY_Plus,
			MovementFormula.rY_Minus,
			MovementFormula.rZ_Plus,
			MovementFormula.rZ_Minus,
		};
		TheComponentVM = new( ) { DefaultMovements = DefaultMovements };
	}

	public Component Component {
		set {
			Connection = value.Connection;
			TheComponentVM.Component = value;
		}
	}

	public bool IsComponentValid {
		get {
			if( TheComponentVM.Name == "" || TheComponentVM.Name == null || TheComponentVM.MovementFormula == null ) {
				return false;
			}
			return true;
		}
	}

	#region 属性和字段
	private OCCCanvas? canvas;
	[ObservableProperty]
	private ComponentPropertiesViewModel theComponentVM;

	public ObservableCollection<Trsf> Connection { get; set; }


	#endregion

	/// <summary>
	/// 可选为父部件的部件集合
	/// </summary>
	public ObservableCollection<Component> Components { protected get; set; }

	/// <summary>
	/// 可选的运动方向
	/// </summary>
	public List<MovementFormula> DefaultMovements { get; }

	public Visibility CreateComponentState { get; set; }

	[ObservableProperty]
	private bool isAddToWorkSpace;

	[RelayCommand(CanExecute = nameof(IsComponentValid))]
	private void CreateComponent( ) {
		App.Current.CommandManager.Execute(new CreateComponentCommand( ), (TheComponentVM.TheComponent, IsAddToWorkSpace));
	}

	public Visibility SaveComponentState { get; set; }

	[RelayCommand]
	private void SaveComponent( ) {
		//App.Current.CommandManager.Execute(new SaveComponentCommand(), component);
		TheComponentVM.OCCFinilize( );
	}

	[RelayCommand]
	private void CancerComponent( ) {
		WeakReferenceMessenger.Default.Send(new PropertiesUIFinishedMessage( ));
		TheComponentVM.OCCFinilize( );
	}

	public void OCCFinilize( ) {
		TheComponentVM.OCCFinilize( );
	}
}
