using System;
using System.Windows.Input;

using IMKernelUI.Interfaces;

using log4net;

namespace PipeBendingUI.Command;

public abstract class RibbonControlUtilCommand:ICommand {
	private static readonly ILog log = LogManager.GetLogger(typeof(RibbonControlUtilCommand));

	public abstract string Description { get; protected set; }

	public event EventHandler? CanExecuteChanged;

	public abstract bool CanExecute( object? parameter );

	public abstract void Execute( object? parameter );
}

public abstract class RibbonControlUtilUndoCommand:RibbonControlUtilCommand, IUndoCommand {
	private static readonly ILog log = LogManager.GetLogger(typeof(RibbonControlUtilUndoCommand));

	public void NotifyCanExecuteChanged( ) {
		throw new NotImplementedException( );
	}

	public abstract void Undo( );
}
