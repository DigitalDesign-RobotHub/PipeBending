using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Utils.DPI;
using IMKernel.Model;

namespace PipeBendingUI.Singleton;

public class StaticResourceManager
{
    public Dictionary<string, Object> Robots = new();
    public Dictionary<string, Object> Models = new();
    public Dictionary<string, Object> Pipes = new();
    public ObservableCollection<Component> Components = new() { new("世界原点") };
    public Dictionary<string, Object> ProgramTemplates = new();
}
