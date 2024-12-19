using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using DevExpress.Map.Kml.Model;
using DevExpress.Xpf.CodeView;
using DevExpress.Xpf.Reports.UserDesigner.ChartDesigner.Native;
using IMKernel.Robotics;
using log4net;
using PipeBendingUI.Model;

namespace PipeBendingUI.ViewModel;

/// <summary>
/// 工作空间树形结构元素
/// </summary>
public partial class WorkSpaceTreeElement : ObservableObject
{
    [ObservableProperty]
    private string name = "";

    [ObservableProperty]
    private WorkSpaceElementType type = WorkSpaceElementType.None;

    [ObservableProperty]
    private int parentID;

    public int ID => GetHashCode();

    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private bool isSelected;

    public object? DataContext { get; set; }
}

/// <summary>
/// 工作空间树形结构视图模型
/// </summary>
public partial class WorkSpaceTreeViewModel : ObservableObject
{
    private static readonly ILog log = LogManager.GetLogger(typeof(WorkSpaceTreeViewModel));

    private static WorkSpace CurrentWorkSpace =>
        App.Current.WorkSpaceContextManager.CurrentWorkSpace;

    public WorkSpaceTreeViewModel()
    {
        try
        {
            // 注册工作空间变更消息
            WeakReferenceMessenger.Default.Register<WorkSpaceChangedMessage>(
                this,
                OnWorkSpaceChanged
            );

            // 生成测试数据
            GenerateTestData();
        }
        catch (Exception ex)
        {
            log.Error("WorkSpaceTreeViewModel 初始化失败", ex);
            throw;
        }
    }

    /// <summary>
    /// 默认树形结构
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<WorkSpaceTreeElement> defaultTreeNodes = new();

    /// <summary>
    /// 分类视图树形结构
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<WorkSpaceTreeElement> compactTreeNodes = new();

    /// <summary>
    /// 选中的节点
    /// </summary>
    [ObservableProperty]
    private WorkSpaceTreeElement? selectedNode;

    /// <summary>
    /// 监听选中节点变化
    /// </summary>
    /// <param name="value"></param>
    partial void OnSelectedNodeChanged(WorkSpaceTreeElement? value)
    {
        try
        {
            if (value != null)
            {
                log.Info($"选中节点: {value.Name}, Type: {value.Type}");
                // 这里可以处理节点选中后的逻辑
                // 例如：发送消息通知其他组件
                WeakReferenceMessenger.Default.Send(new TreeNodeSelectedMessage(value));
            }
        }
        catch (Exception ex)
        {
            log.Error("处理节点选中失败", ex);
        }
    }

    /// <summary>
    /// 消息：选中节点
    /// </summary>
    public class TreeNodeSelectedMessage
    {
        public WorkSpaceTreeElement Node { get; }

        public TreeNodeSelectedMessage(WorkSpaceTreeElement node)
        {
            Node = node;
        }
    }

    /// <summary>
    /// 处理工作空间变更消息
    /// </summary>
    private void OnWorkSpaceChanged(object recipient, WorkSpaceChangedMessage message)
    {
        try
        {
            log.Info("更新 WorkSpaceTree");
            UpdateTreeStructures();
        }
        catch (Exception ex)
        {
            log.Error("WorkSpaceTree 更新失败", ex);
        }
    }

    /// <summary>
    /// 更新树形结构
    /// </summary>
    private void UpdateTreeStructures()
    {
        try
        {
            // 清空现有树结构
            DefaultTreeNodes.Clear();
            CompactTreeNodes.Clear();

            // 解析工作空间并生成树结构
            ParseWorkSpaceToDefaultTree();
            ParseWorkSpaceToCompactTree();
            log.Info("更新WorkSpaceTree");
        }
        catch (Exception ex)
        {
            log.Error("工作空间树结构更新失败", ex);
            throw;
        }
    }

    /// <summary>
    /// 解析工作空间生成默认树结构
    /// </summary>
    private void ParseWorkSpaceToDefaultTree()
    {
        var root = new WorkSpaceTreeElement
        {
            Name = "工作空间",
            Type = WorkSpaceElementType.None,
            DataContext = CurrentWorkSpace
        };

        // 解析和添加零部件
        if (CurrentWorkSpace.Components.Any() == true)
        {
            foreach (var axis in CurrentWorkSpace.Components)
            {
                var axisNode = new WorkSpaceTreeElement
                {
                    Name = axis.Name,
                    Type = WorkSpaceElementType.Component,
                    DataContext = axis,
                    ParentID = root.ID
                };
                DefaultTreeNodes.Add(axisNode);
            }
        }

        // 解析和添加机器人
        if (CurrentWorkSpace.Robots?.Any() == true)
        {
            foreach (var robot in CurrentWorkSpace.Robots)
            {
                var robotNode = new WorkSpaceTreeElement
                {
                    Name = robot.Name,
                    Type = WorkSpaceElementType.Robot,
                    DataContext = robot,
                    ParentID = root.ID
                };
                DefaultTreeNodes.Add(robotNode);
            }
        }

        // 解析和添加干涉检查
        if (CurrentWorkSpace.Interferences?.Any() == true)
        {
            foreach (var interference in CurrentWorkSpace.Interferences)
            {
                var interferenceNode = new WorkSpaceTreeElement
                {
                    Name = interference.Name,
                    Type = WorkSpaceElementType.Interference,
                    DataContext = interference,
                    ParentID = root.ID
                };
                DefaultTreeNodes.Add(interferenceNode);
            }
        }

        DefaultTreeNodes.Add(root);
    }

    /// <summary>
    /// 解析工作空间生成分组树结构
    /// </summary>
    private void ParseWorkSpaceToCompactTree()
    {
        var root = new WorkSpaceTreeElement { Name = "分类视图", Type = WorkSpaceElementType.None };
        CompactTreeNodes.Add(root);

        // 创建分类节点
        var axisGroup = new WorkSpaceTreeElement
        {
            Name = "轴系",
            Type = WorkSpaceElementType.Group,
            ParentID = root.ID
        };
        var robotGroup = new WorkSpaceTreeElement
        {
            Name = "机器人",
            Type = WorkSpaceElementType.Group,
            ParentID = root.ID
        };
        var ifGroup = new WorkSpaceTreeElement
        {
            Name = "干涉检查",
            Type = WorkSpaceElementType.Group,
            ParentID = root.ID
        };

        // 按类型分组添加项目
        if (CurrentWorkSpace.Components?.Any() == true)
        {
            foreach (var component in CurrentWorkSpace.Components)
            {
                var axisNode = new WorkSpaceTreeElement
                {
                    Name = component.Name,
                    Type = WorkSpaceElementType.Component,
                    DataContext = component,
                    ParentID = axisGroup.ID
                };
                CompactTreeNodes.Add(axisNode);
            }
            CompactTreeNodes.Add(axisGroup);
        }

        if (CurrentWorkSpace.Robots?.Any() == true)
        {
            foreach (var robot in CurrentWorkSpace.Robots)
            {
                var robotNode = new WorkSpaceTreeElement
                {
                    Name = robot.Name,
                    Type = WorkSpaceElementType.Robot,
                    DataContext = robot,
                    ParentID = robotGroup.ID
                };
                CompactTreeNodes.Add(robotNode);
            }
            CompactTreeNodes.Add(robotGroup);
        }

        if (CurrentWorkSpace.Interferences?.Any() == true)
        {
            foreach (var interference in CurrentWorkSpace.Interferences)
            {
                WorkSpaceTreeElement ifNode =
                    new()
                    {
                        Name = interference.Name,
                        Type = WorkSpaceElementType.Interference,
                        DataContext = interference,
                        ParentID = ifGroup.ID
                    };
                CompactTreeNodes.Add(ifNode);
            }
            CompactTreeNodes.Add(ifGroup);
        }
    }

    #region Test

    /// <summary>
    /// 生成测试数据
    /// </summary>
    public void GenerateTestData()
    {
        try
        {
            // 修改当前WorkSpace，添加测试数据
            //CurrentWorkSpace.Components;

            CurrentWorkSpace.Robots.AddRange(
                [
                    new(new("机器人1", RobotType.Custom)),
                    new(new("机器人2", RobotType.Custom)),
                    new(new("机器人3", RobotType.Custom)),
                    new(new("机器人4", RobotType.Custom))
                ]
            );

            CurrentWorkSpace.Interferences.AddRange(
                [
                    new() { Name = "干涉检查1" },
                    new() { Name = "干涉检查2" },
                    new() { Name = "干涉检查3" },
                    new() { Name = "干涉检查4" }
                ]
            );

            // 触发树结构更新
            UpdateTreeStructures();

            log.Info("测试数据生成成功");
        }
        catch (Exception ex)
        {
            log.Error("测试数据生成失败", ex);
            throw;
        }
    }
    #endregion
}
