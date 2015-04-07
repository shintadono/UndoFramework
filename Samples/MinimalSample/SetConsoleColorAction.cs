using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndoFramework;

namespace MinimalSample
{
	class SetConsoleColorAction : AbstractAction
	{
		public SetConsoleColorAction(ConsoleColor newColor)
		{
			color=newColor;
		}

		ConsoleColor color;
		ConsoleColor oldColor;

		public override string Name
		{
			get
			{
				return "Set console color";
			}
		}

		protected override void ExecuteCore()
		{
			oldColor=Console.ForegroundColor;
			Console.ForegroundColor=color;
		}

		protected override void UnExecuteCore()
		{
			Console.ForegroundColor=oldColor;
		}
	}
}
