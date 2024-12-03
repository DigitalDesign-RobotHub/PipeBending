using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using IMKernel.Robotics;

namespace PipeBending.Model;

public enum WorkSpaceElementType
{
    None = -1,
    Component,
    Robot,
    Interference
}

public class WorkSpaceElement
{
    public string Name { get; set; } = ""; // 节点名称
    public WorkSpaceElementType Type { get; set; }
    public ObservableCollection<WorkSpaceElement> Children { get; set; } = new();
}

public class RobotElement : WorkSpaceElement
{
    public RobotElement(SixArmRobot robot)
        : base()
    {
        Robot = robot;
        Type = WorkSpaceElementType.Robot;
    }

    public SixArmRobot Robot { get; set; }
    public new string Name => Robot.Name;
}

public class ComponentElement : WorkSpaceElement
{
    public ComponentElement()
        : base()
    {
        Type = WorkSpaceElementType.Component;
    }
}

public class InterferencesElement : WorkSpaceElement
{
    public InterferencesElement()
        : base()
    {
        Type = WorkSpaceElementType.Interference;
    }
}

// 用于MVVM消息通信的消息类
public record WorkSpaceChangedMessage(WorkSpace NewWorkSpace);

public partial class WorkSpace : ObservableObject, ICloneable
{
    public ObservableCollection<ComponentElement> Components { get; set; } = [];

    public ObservableCollection<RobotElement> Robots { get; set; } = [];

    public ObservableCollection<WorkSpaceElement> Interferences { get; set; } = [];

    public object Clone()
    {
        return new();
    }
}
