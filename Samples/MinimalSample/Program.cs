using System;
using UndoFramework;

namespace MinimalSample
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Original color");

			SetConsoleColor(ConsoleColor.Green);
			Console.WriteLine("New color");

			actionManager.Undo();
			Console.WriteLine("Old color again");

			using(Transaction.Create(actionManager, "Change colors"))
			{
				SetConsoleColor(ConsoleColor.Red); // you never see Red
				Console.WriteLine("Still didn't change to Red because of lazy evaluation");
				SetConsoleColor(ConsoleColor.Blue);
			}

			Console.WriteLine("Changed two colors at once");

			actionManager.Undo();
			Console.WriteLine("Back to original");

			actionManager.Redo();
			Console.WriteLine("Blue again");
			Console.ReadKey();
		}

		static void SetConsoleColor(ConsoleColor color)
		{
			SetConsoleColorAction action=new SetConsoleColorAction(color);
			actionManager.RecordAction(action);
		}

		static ActionManager actionManager=new ActionManager();
	}
}
