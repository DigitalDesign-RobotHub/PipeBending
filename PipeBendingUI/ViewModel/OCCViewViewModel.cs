using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IMKernel.Visualization;

namespace PipeBendingUI.ViewModel;

[ObservableObject]
public partial class OCCViewViewModel
{
    public readonly OCCCanvas occCanvas;

    public OCCViewViewModel(OCCCanvas canvas)
    {
        // 初始化 ViewModel 或需要的资源
        occCanvas = canvas;
    }
}
