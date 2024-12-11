using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using IMKernel.Interfaces;
using IMKernel.Model;
using log4net;
using PipeBendingUI.Message;
using PipeBendingUI.ViewModel;
using CommandManager = PipeBendingUI.Singleton.CommandManager;

namespace PipeBendingUI.Command;

public abstract class RibbonControlUtilCommand : ICommand
{
    private static readonly ILog log = LogManager.GetLogger(typeof(RibbonControlUtilCommand));

    public abstract string Description { get; protected set; }

    public event EventHandler? CanExecuteChanged;

    public abstract bool CanExecute(object? parameter);

    public abstract void Execute(object? parameter);
}

public abstract class RibbonControlUtilUndoCommand : RibbonControlUtilCommand, IUndoCommand
{
    private static readonly ILog log = LogManager.GetLogger(typeof(RibbonControlUtilUndoCommand));

    public abstract void Undo();
}
