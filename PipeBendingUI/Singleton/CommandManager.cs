using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using IMKernel.Interfaces;
using log4net;

namespace PipeBendingUI.Singleton;

public class CommandManager : ICommandManager
{
    private static readonly ILog log = LogManager.GetLogger(typeof(CommandManager));
    private readonly Stack<IUndoCommand> UndoStack;

    public CommandManager()
    {
        UndoStack = new Stack<IUndoCommand>();
    }

    public void Execute(IUndoCommand command, object? parameter = null)
    {
        try
        {
            command.Execute(parameter);
        }
        catch (Exception e)
        {
            log.Error(e);
            MessageBox.Show($"！！执行失败！！\n{e.Message}");
        }
        UndoStack.Push(command);
    }

    public void Execute(ICommand command, object? parameter = null)
    {
        try
        {
            command.Execute(parameter);
        }
        catch (Exception e)
        {
            log.Error(e);
            MessageBox.Show($"！！执行失败！！\n{e.Message}");
        }
    }

    public void Undo()
    {
        if (UndoStack.Count == 0)
        {
            return;
        }

        var lastCommand = UndoStack.Pop();
        log.Debug($"Undo: {lastCommand.Description}");
        lastCommand.Undo();
    }
}
