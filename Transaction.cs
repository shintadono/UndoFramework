using System;
using System.Collections.Generic;

namespace UndoFramework
{
	public sealed class Transaction : IAction, IDisposable
	{
		readonly List<IAction> Actions;
		readonly ActionManager ActionManager;
		bool Aborted { get; set; }

		public bool AllowToMergeWithPrevious { get; set; }
		public bool IsDelayed { get; set; }

		Transaction(ActionManager actionManager, bool delayed)
		{
			Actions=new List<IAction>();
			ActionManager=actionManager;
			actionManager.OpenTransaction(this);
			IsDelayed=delayed;
		}

		public static Transaction Create(ActionManager actionManager, bool delayed)
		{
			if(actionManager==null) throw new ArgumentNullException("actionManager");

			return new Transaction(actionManager, delayed);
		}

		/// <summary>
		/// By default, the actions are delayed and executed only after
		/// the top-level transaction commits.
		/// </summary>
		/// <remarks>
		/// Make sure to dispose of the transaction once you're done - it will actually call Commit for you.
		/// </remarks>
		/// <example>
		/// Recommended usage: using (Transaction.Create(actionManager)) { DoStuff(); }
		/// </example>
		public static Transaction Create(ActionManager actionManager)
		{
			return Create(actionManager, true);
		}

		#region IAction implementation
		public void Execute()
		{
			if(!IsDelayed)
			{
				IsDelayed=true;
				return;
			}

			for(int i=0; i<Actions.Count; i++) Actions[i].Execute();
		}

		public void UnExecute()
		{
			for(int i=Actions.Count-1; i>=0; i--) Actions[i].UnExecute();
		}

		public bool CanExecute()
		{
			for(int i=0; i<Actions.Count; i++)
			{
				if(!Actions[i].CanExecute()) return false;
			}
			return true;
		}

		public bool CanUnExecute()
		{
			for(int i=Actions.Count-1; i>=0; i--)
			{
				if(!Actions[i].CanUnExecute()) return false;
			}
			return true;
		}

		public bool TryToMerge(IAction followingAction)
		{
			return false;
		}
		#endregion

		public void Commit()
		{
			ActionManager.CommitTransaction();
		}

		public void Rollback()
		{
			ActionManager.RollBackTransaction();
			Aborted=true;
		}

		public void Dispose()
		{
			if(!Aborted) Commit();
		}

		public void Add(IAction actionToAppend)
		{
			if(actionToAppend==null) throw new ArgumentNullException("actionToAppend");

			Actions.Add(actionToAppend);
		}

		public bool HasActions()
		{
			return Actions.Count!=0;
		}

		public void Remove(IAction actionToCancel)
		{
			if(actionToCancel==null) throw new ArgumentNullException("actionToCancel");

			Actions.Remove(actionToCancel);
		}
	}
}
