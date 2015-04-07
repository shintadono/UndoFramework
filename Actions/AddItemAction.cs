using System;

namespace UndoFramework
{
	public class AddItemAction<T> : AbstractAction
	{
		public AddItemAction(Action<T> adder, Action<T> remover, T item)
		{
			Adder=adder;
			Remover=remover;
			Item=item;
		}

		public Action<T> Adder { get; set; }
		public Action<T> Remover { get; set; }
		public T Item { get; set; }

		public override string Name
		{
			get
			{
				return "Add item";
			}
		}

		protected override void ExecuteCore()
		{
			Adder(Item);
		}

		protected override void UnExecuteCore()
		{
			Remover(Item);
		}
	}
}
