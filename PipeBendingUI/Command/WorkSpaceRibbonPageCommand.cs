using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using IMKernel.Interfaces;
using log4net;
using PipeBendingUI.Message;

namespace PipeBendingUI.Command;

#region 抽象类

public abstract class WorkSpaceRibbonPageCommand : ICommand
{
    private static readonly ILog log = LogManager.GetLogger(typeof(WorkSpaceRibbonPageCommand));

    public abstract string Description { get; protected set; }

    public event EventHandler? CanExecuteChanged;

    public abstract bool CanExecute(object? parameter);

    public abstract void Execute(object? parameter);
}

public abstract class WorkSpaceRibbonPageUndoCommand : WorkSpaceRibbonPageCommand, IUndoCommand
{
    private static readonly ILog log = LogManager.GetLogger(typeof(WorkSpaceRibbonPageUndoCommand));

    public abstract void Undo();
}

#endregion

public partial class CreateNewComponent : WorkSpaceRibbonPageCommand
{
    public override string Description { get; protected set; } = "创建新部件";

    public override bool CanExecute(object? parameter)
    {
        return true;
    }

    public override void Execute(object? parameter)
    {
        WeakReferenceMessenger.Default.Send(new ComponentChangedMessage(null));
    }
}
