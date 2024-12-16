using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using IMKernel.Model;
using IMKernel.Utils;
using IMKernel.ViewModel;
using PipeBendingUI.Command;
using PipeBendingUI.Message;

namespace PipeBendingUI.ViewModel;

[ObservableObject]
public partial class ComponentPropertiesViewModel
{
    public ComponentPropertiesViewModel()
    {
        TheTrsfViewModel = new TrsfViewModel();
        CreateComponentState = Visibility.Visible;
        SaveComponentState = Visibility.Collapsed;
    }

    [ObservableProperty]
    public TrsfViewModel theTrsfViewModel;

    [ObservableProperty]
    private Component component;

    [ObservableProperty]
    private Component selectedParentComponent;

    [ObservableProperty]
    private MovementFormula movement;

    partial void OnSelectedParentComponentChanged(Component value)
    {
        Component.Parent = value;
    }

    public ObservableCollection<Component> Components =>
        App.Current.StaticResourceManager.Components;

    partial void OnComponentChanged(Component value)
    {
        TheTrsfViewModel.TheTrsf = value.Base;
    }

    public Visibility CreateComponentState { get; set; }

    [RelayCommand]
    private void CreateComponent()
    {
        App.Current.CommandManager.Execute(new CreateComponentCommand(), component);
    }

    public Visibility SaveComponentState { get; set; }

    [RelayCommand]
    private void SaveComponent()
    {
        App.Current.CommandManager.Execute(new SaveComponentCommand(), component);
    }

    [RelayCommand]
    private void CancerComponent()
    {
        WeakReferenceMessenger.Default.Send(new PropertiesUIFinishedMessage());
    }
}
