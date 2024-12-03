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
using IMKernel.Robotics;
using log4net;
using PipeBending.Model;

namespace PipeBending.ViewModel;

/// <summary>
/// 工作空间树形结构元素
/// </summary>
public partial class WorkSpaceTreeElement : ObservableObject
{
    /// <summary>
    /// 元素名称
    /// </summary>
    [ObservableProperty]
    private string name = "";

    /// <summary>
    /// 元素类型
    /// </summary>
    [ObservableProperty]
    private WorkSpaceElementType type = WorkSpaceElementType.None;

    /// <summary>
    /// 父节点ID
    /// </summary>
    [ObservableProperty]
    private int parentID;

    /// <summary>
    /// ID
    /// </summary>
    public int ID => GetHashCode();

    /// <summary>
    /// 子节点集合
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<WorkSpaceTreeElement> children = new();

    /// <summary>
    /// 是否展开
    /// </summary>
    [ObservableProperty]
    private bool isExpanded;

    /// <summary>
    /// 是否选中
    /// </summary>
    [ObservableProperty]
    private bool isSelected;

    /// <summary>
    /// 关联的数据对象
    /// </summary>
    public object? DataContext { get; set; }
}

/// <summary>
/// 工作空间树形结构视图模型
/// </summary>
public partial class WorkSpaceTreeViewModel : ObservableObject
{
    private static readonly ILog log = LogManager.GetLogger(typeof(WorkSpaceTreeViewModel));

    /// <summary>
    /// 当前工作空间
    /// </summary>
    [ObservableProperty]
    private WorkSpace currentWorkSpace;

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

    // 监听选中节点变化
    partial void OnSelectedNodeChanged(WorkSpaceTreeElement? value)
    {
        try
        {
            if (value != null)
            {
                log.Info($"Selected node changed: {value.Name}, Type: {value.Type}");
                // 这里可以处理节点选中后的逻辑
                // 例如：发送消息通知其他组件
                WeakReferenceMessenger.Default.Send(new TreeNodeSelectedMessage(value));
            }
        }
        catch (Exception ex)
        {
            log.Error("Error handling node selection change", ex);
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

    partial void OnCurrentWorkSpaceChanged(WorkSpace value)
    {
        UpdateTreeStructures();
    }

    public WorkSpaceTreeViewModel()
    {
        try
        {
            CurrentWorkSpace = App.Current.WorkSpaceContextManager.CurrentWorkSpace;

            // 注册工作空间变更消息
            WeakReferenceMessenger.Default.Register<WorkSpaceChangedMessage>(
                this,
                OnWorkSpaceChanged
            );

            // 生成测试数据
            GenerateTestData();

            log.Info("WorkSpaceTreeViewModel initialized successfully");
        }
        catch (Exception ex)
        {
            log.Error("Failed to initialize WorkSpaceTreeViewModel", ex);
            throw;
        }
    }

    /// <summary>
    /// 处理工作空间变更消息
    /// </summary>
    private void OnWorkSpaceChanged(object recipient, WorkSpaceChangedMessage message)
    {
        try
        {
            CurrentWorkSpace = message.NewWorkSpace;
            log.Info("Workspace changed, tree structures updated");
        }
        catch (Exception ex)
        {
            log.Error("Failed to handle workspace change", ex);
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

            log.Info("Tree structures updated successfully");
        }
        catch (Exception ex)
        {
            log.Error("Failed to update tree structures", ex);
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
                root.Children.Add(axisNode);
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
                root.Children.Add(robotNode);
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
                root.Children.Add(interferenceNode);
                DefaultTreeNodes.Add(interferenceNode);
            }
        }

        DefaultTreeNodes.Add(root);
    }

    /// <summary>
    /// 解析工作空间生成压缩树结构
    /// </summary>
    private void ParseWorkSpaceToCompactTree()
    {
        var root = new WorkSpaceTreeElement { Name = "分类视图", Type = WorkSpaceElementType.None };

        // 创建分类节点
        var axesNode = new WorkSpaceTreeElement
        {
            Name = "轴系",
            Type = WorkSpaceElementType.Component
        };
        var robotsNode = new WorkSpaceTreeElement
        {
            Name = "机器人",
            Type = WorkSpaceElementType.Robot
        };
        var interferencesNode = new WorkSpaceTreeElement
        {
            Name = "干涉检查",
            Type = WorkSpaceElementType.Interference
        };

        // 按类型分组添加项目
        if (CurrentWorkSpace.Components?.Any() == true)
        {
            foreach (var axis in CurrentWorkSpace.Components)
            {
                axesNode.Children.Add(
                    new WorkSpaceTreeElement
                    {
                        Name = axis.Name,
                        Type = WorkSpaceElementType.Component,
                        DataContext = axis
                    }
                );
            }
            root.Children.Add(axesNode);
        }

        if (CurrentWorkSpace.Robots?.Any() == true)
        {
            foreach (var robot in CurrentWorkSpace.Robots)
            {
                robotsNode.Children.Add(
                    new WorkSpaceTreeElement
                    {
                        Name = robot.Name,
                        Type = WorkSpaceElementType.Robot,
                        DataContext = robot
                    }
                );
            }
            root.Children.Add(robotsNode);
        }

        if (CurrentWorkSpace.Interferences?.Any() == true)
        {
            foreach (var interference in CurrentWorkSpace.Interferences)
            {
                interferencesNode.Children.Add(
                    new WorkSpaceTreeElement
                    {
                        Name = interference.Name,
                        Type = WorkSpaceElementType.Interference,
                        DataContext = interference
                    }
                );
            }
            root.Children.Add(interferencesNode);
        }

        CompactTreeNodes.Add(root);
    }

    /// <summary>
    /// 生成测试数据
    /// </summary>
    public void GenerateTestData()
    {
        try
        {
            // 修改当前WorkSpace，添加测试数据
            CurrentWorkSpace.Components = new()
            {
                new() { Name = "轴系1" },
                new() { Name = "轴系2" }
            };

            CurrentWorkSpace.Robots = new()
            {
                new(new("机器人1", RobotType.Custom)),
                new(new("机器人2", RobotType.Custom))
            };

            CurrentWorkSpace.Interferences = new()
            {
                new() { Name = "干涉检查1" },
                new() { Name = "干涉检查2" }
            };

            // 触发树结构更新
            UpdateTreeStructures();

            log.Info("Test data generated successfully");
        }
        catch (Exception ex)
        {
            log.Error("Failed to generate test data", ex);
            throw;
        }
    }
}
