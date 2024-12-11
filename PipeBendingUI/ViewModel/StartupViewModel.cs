using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.CodeParser;

namespace PipeBendingUI.ViewModel;

public enum StartupResult
{
    MainWindow,
    Todo,
    Close
}

public class StartupViewModel
{
    public StartupViewModel() { }

    /// <summary>
    /// 窗口显示后延迟加载
    /// </summary>
    internal void DeferredInit()
    {
        //+ 加载最近使用文件列表（MRU）
        //MruList = _LoadMRU();
        //RaisePropertyChanged(nameof(MruList));

        //+ 确定样例目录路径
        //SamplesDirectory = Path.Combine(PathUtils.GetAppExecutableDirectory(), "Samples");
        //if (!Directory.Exists(SamplesDirectory)) {
        //// Try repository structure
        //SamplesDirectory = Path.GetFullPath(Path.Combine(PathUtils.GetAppExecutableDirectory(), @"..", "..", "Data", "Samples"));
        //if (!Directory.Exists(SamplesDirectory)) {
        //SamplesDirectory = null; // Disable command
        //}
        //}
    }
}
