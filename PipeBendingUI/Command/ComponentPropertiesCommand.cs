using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Utils.Extensions;
using IMKernel.Interfaces;
using IMKernel.Model;
using log4net;

namespace PipeBendingUI.Command;

public abstract class ComponentPropertiesCommand : ICommand
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ComponentPropertiesCommand));

    public abstract string Description { get; protected set; }

    public event EventHandler? CanExecuteChanged;

    public abstract bool CanExecute(object? parameter);

    public abstract void Execute(object? parameter);
}

public abstract class ComponentPropertiesUndoCommand : ComponentPropertiesCommand, IUndoCommand
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ComponentPropertiesUndoCommand));

    public abstract void Undo();
}

public class CreateComponentCommand : IUndoCommand
{
    private static ObservableCollection<Component> Components =>
        App.Current.StaticResourceManager.Components;
    public string Description => "创建新部件";

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        //todo 输入检查
        throw new NotImplementedException();
    }

    public void Execute(object? parameter = null)
    {
        if (parameter is Component com)
        {
            Components.Add(com);
        }
    }

    public void Undo()
    {
        if (Components.Count <= 0)
        {
            return;
        }
        Components.RemoveAt(Components.Count - 1);
    }
}

public class SaveComponentCommand : IUndoCommand
{
    public string Description => "保存部件";

    public event EventHandler? CanExecuteChanged;
    private static ObservableCollection<Component> Components =>
        App.Current.StaticResourceManager.Components;

    public bool CanExecute(object? parameter)
    {
        //todo 输入检查
        throw new NotImplementedException();
    }

    public void Execute(object? parameter = null)
    {
        //todo 保存的逻辑
        throw new NotImplementedException();
    }

    private Component lastComponent;
    private Component newComponent;

    public void Undo()
    {
        if (!Components.Contains(newComponent))
        {
            return;
        }
        int index = Components.IndexOf(newComponent);
        if (index < 0)
        {
            return;
        }
        Components[index] = lastComponent;
    }
}

//public class CancerComponentCommand : IUndoCommand
//{
//    public string Description => throw new NotImplementedException();

//    public event EventHandler? CanExecuteChanged;

//    public bool CanExecute(object? parameter)
//    {
//        throw new NotImplementedException();
//    }

//    public void Execute(object? parameter = null)
//    {
//        throw new NotImplementedException();
//    }

//    public void Undo()
//    {
//        throw new NotImplementedException();
//    }
//}
