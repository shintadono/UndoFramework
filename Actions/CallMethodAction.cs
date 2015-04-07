﻿using System;

namespace UndoFramework
{
	public class CallMethodAction : AbstractAction
	{
		public CallMethodAction(Action execute, Action unexecute)
		{
			ExecuteDelegate=execute;
			UnexecuteDelegate=unexecute;
		}

		public Action ExecuteDelegate { get; set; }
		public Action UnexecuteDelegate { get; set; }

		public override string Name
		{
			get
			{
				return "Call method";
			}
		}

		protected override void ExecuteCore()
		{
			if(ExecuteDelegate!=null)
			{
				ExecuteDelegate();
			}
		}

		protected override void UnExecuteCore()
		{
			if(UnexecuteDelegate!=null)
			{
				UnexecuteDelegate();
			}
		}
	}
}
