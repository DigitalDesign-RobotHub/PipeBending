using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using IMKernel.Model;
using IMKernel.Robotics;
using log4net;

namespace PipeBendingUI.Model;

public enum WorkSpaceElementType
{
    None = -1,
    Group = 0,
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
    public ComponentElement(Component component)
        : base()
    {
        Type = WorkSpaceElementType.Component;
        Name = component.Name;
        Component = component;
    }

    Component Component { get; }
}

public class InterferencesElement : WorkSpaceElement
{
    public InterferencesElement()
        : base()
    {
        Type = WorkSpaceElementType.Interference;
    }
}

#region Message
public record WorkSpaceChangedMessage(WorkSpace NewWorkSpace);
#endregion

public partial class WorkSpace : ObservableObject
{
    private static readonly ILog log = LogManager.GetLogger(typeof(WorkSpace));

    public WorkSpace()
    {
        // 订阅集合的变化事件
        Components.CollectionChanged += OnCollectionChanged;
        Robots.CollectionChanged += OnCollectionChanged;
        Interferences.CollectionChanged += OnCollectionChanged;
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // 发送 WorkSpaceChangedMessage 消息
        WeakReferenceMessenger.Default.Send(new WorkSpaceChangedMessage(this));
        log.Info("更新工作空间");
    }

    public ObservableCollection<ComponentElement> Components { get; } = [];

    public ObservableCollection<RobotElement> Robots { get; } = [];

    public ObservableCollection<WorkSpaceElement> Interferences { get; } = [];
}
