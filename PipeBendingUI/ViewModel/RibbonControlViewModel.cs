using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IMKernel.Robotics;

namespace PipeBending.ViewModel;

public partial class RibbonControlViewModel : ObservableObject
{
    [RelayCommand]
    private void CreateNewRobot()
    {
        FanucRobot robot = new("testRobot");
        App.Current.WorkSpaceContextManager.CurrentWorkSpace.Robots.Add(new(robot));
    }
}
