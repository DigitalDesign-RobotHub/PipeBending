using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Grid.TreeList;
using log4net;
using PipeBendingUI.ViewModel;

namespace PipeBendingUI.View
{
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
                    log.Info($"Node {element.Name} expanded");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error handling node expand", ex);
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
                    log.Info($"Node {element.Name} collapsed");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error handling node collapse", ex);
            }
        }
    }
}
