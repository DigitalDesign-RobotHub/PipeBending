using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using log4net;
using PipeBendingUI.ViewModel;

namespace PipeBendingUI.View;

/// <summary>
/// WorkSpaceTree.xaml 的交互逻辑
/// </summary>
public partial class WorkSpaceTreeUserControl : UserControl
{
    private static readonly ILog log = LogManager.GetLogger(typeof(WorkSpaceTreeUserControl));

    public WorkSpaceTreeUserControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 处理节点展开事件
    /// </summary>
    private void TreeViewControl_NodeExpanded(object sender, TreeViewNodeEventArgs e)
    {
        try
        {
            if (e.Node.Content is WorkSpaceTreeElement element)
            {
                element.IsExpanded = true;
                log.Info($"展开节点: {element.Name}");
            }
        }
        catch (Exception ex)
        {
            log.Error("节点展开失败", ex);
        }
    }

    /// <summary>
    /// 处理节点折叠事件
    /// </summary>
    private void TreeViewControl_NodeCollapsed(object sender, TreeViewNodeEventArgs e)
    {
        try
        {
            if (e.Node.Content is WorkSpaceTreeElement element)
            {
                element.IsExpanded = false;
                log.Info($"折叠节点: {element.Name}");
            }
        }
        catch (Exception ex)
        {
            log.Error("节点折叠失败", ex);
        }
    }

    //private void TreeViewControl_StartRecordDrag(object sender, StartRecordDragEventArgs e)
    //{
    //    // 获取拖动的节点数据
    //    var draggedNode = e.Records?.FirstOrDefault() as WorkSpaceTreeElement;
    //    if (draggedNode != null)
    //    {
    //        // 设置拖动数据
    //        e.Data.SetData(typeof(WorkSpaceTreeElement), draggedNode);
    //        Debug.WriteLine($"Drag started: {draggedNode.Type}");
    //    }
    //}

    //private void TreeViewControl_DragRecordOver(object sender, DragRecordOverEventArgs e)
    //{
    //    // 获取拖动的节点和目标节点
    //    var draggedNode = e.Data?.GetData(typeof(WorkSpaceTreeElement)) as WorkSpaceTreeElement;
    //    var targetNode = e.TargetRecord as WorkSpaceTreeElement;
    //    Debug.WriteLine($"d:{draggedNode} | {targetNode}");
    //    if (draggedNode != null && targetNode != null)
    //    {
    //        Debug.WriteLine($"{draggedNode.Type} | {targetNode.Type}");
    //        // 检查类型是否匹配
    //        if (draggedNode.Type != targetNode.Type)
    //        {
    //            // 阻止拖放
    //            e.Effects = DragDropEffects.None;
    //            e.Handled = true;
    //            Debug.WriteLine("No Drag");
    //            return;
    //        }
    //    }
    //    // 允许拖放
    //    e.Effects = DragDropEffects.Move;
    //    e.Handled = true;
    //    Debug.WriteLine("Drag");
    //}

    //private void TreeViewControl_DropRecord(object sender, DropRecordEventArgs e)
    //{
    //    // 获取拖动的节点和目标节点
    //    var draggedNode = e.Data?.GetData(typeof(WorkSpaceTreeElement)) as WorkSpaceTreeElement;
    //    var targetNode = e.TargetRecord as WorkSpaceTreeElement;

    //    // 如果拖动节点或目标节点为空，或类型不匹配，则不处理
    //    if (draggedNode == null || targetNode == null || draggedNode.Type != targetNode.Type)
    //    {
    //        e.Handled = true;
    //        return;
    //    }

    //    // 获取数据源 (假设你的节点存储在一个 ObservableCollection 中)
    //    var dataSource =
    //        (sender as TreeViewControl)?.ItemsSource as ObservableCollection<WorkSpaceTreeElement>;
    //    if (dataSource == null)
    //    {
    //        e.Handled = true;
    //        return;
    //    }

    //    // 获取拖动节点和目标节点的索引
    //    int draggedIndex = dataSource.IndexOf(draggedNode);
    //    int targetIndex = dataSource.IndexOf(targetNode);

    //    // 如果两个节点在数据源中都存在
    //    if (draggedIndex >= 0 && targetIndex >= 0)
    //    {
    //        // 交换节点顺序
    //        dataSource.Move(draggedIndex, targetIndex);
    //        log.Info($"拖放成功: {draggedNode.NameString} 和 {targetNode.NameString} 交换顺序");
    //    }

    //    e.Handled = true;
    //}
}
