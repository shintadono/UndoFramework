using System;
using System.Collections.Generic;

namespace UndoFramework
{
	/// <summary>
	/// IActionHistory represents a recorded list of actions undertaken by user.
	///
	/// This class implements a usual, linear action sequence. You can move back and forth
	/// changing the state of the respective document. When you move forward, you execute
	/// a respective action, when you move backward, you Undo it (UnExecute).
	///
	/// Implemented through a double linked-list of SimpleHistoryNode objects.
	/// </summary>
	internal class SimpleHistory : IActionHistory
	{
		public SimpleHistory()
		{
			Init();
		}

		#region Events
		public event EventHandler CollectionChanged;
		protected void RaiseUndoBufferChanged()
		{
			EventHandler ev=CollectionChanged;
			if(ev!=null) ev(this, new EventArgs());
		}
		#endregion

		private SimpleHistoryNode mCurrentState=new SimpleHistoryNode();

		/// <summary>
		/// "Iterator" to navigate through the sequence, "Cursor".
		/// </summary>
		public SimpleHistoryNode CurrentState
		{
			get
			{
				return mCurrentState;
			}
			set
			{
				if(value==null) throw new ArgumentNullException("CurrentState");
				mCurrentState=value;
			}
		}

		public SimpleHistoryNode Head { get; set; }

		public IAction LastAction { get; set; }

		/// <summary>
		/// Adds a new action to the tail after current state. If 
		/// there exist more actions after this, they're lost (Garbage Collected).
		/// This is the only method of this class that actually modifies the linked-list.
		/// </summary>
		/// <param name="newAction">Action to be added.</param>
		/// <returns><b>true</b> if action was appended, <b>false</b> if it was merged with the previous one.</returns>
		public bool AppendAction(IAction newAction)
		{
			if(CurrentState.PreviousAction!=null&&CurrentState.PreviousAction.TryToMerge(newAction))
			{
				RaiseUndoBufferChanged();
				return false;
			}
			CurrentState.NextAction=newAction;
			CurrentState.NextNode=new SimpleHistoryNode(newAction, CurrentState);
			return true;
		}

		/// <summary>
		/// All existing Nodes and Actions are garbage collected.
		/// </summary>
		public void Clear()
		{
			Init();
			RaiseUndoBufferChanged();
		}

		private void Init()
		{
			CurrentState=new SimpleHistoryNode();
			Head=CurrentState;
		}

		public IEnumerable<IAction> EnumUndoableActions()
		{
			SimpleHistoryNode current=Head;
			while(current!=null&&current!=CurrentState&&current.NextAction!=null)
			{
				yield return current.NextAction;
				current=current.NextNode;
			}
		}

		public IEnumerable<IAction> RedoableActions
		{
			get
			{
				SimpleHistoryNode current=CurrentState;
				while(current!=null&&current.NextAction!=null)
				{
					yield return current.NextAction;
					current=current.NextNode;
				}
			}
		}

		public IEnumerable<IAction> UndoableActions
		{
			get
			{
				SimpleHistoryNode current=CurrentState;
				while(current!=null&&current.PreviousAction!=null)
				{
					yield return current.PreviousAction;
					current=current.PreviousNode;
				}
			}
		}

		public void MoveForward()
		{
			if(!CanMoveForward)
			{
				throw new InvalidOperationException(
					"History.MoveForward() cannot execute because"
					+" CanMoveForward returned false (the current state"
					+" is the last state in the undo buffer.");
			}
			CurrentState.NextAction.Execute();
			CurrentState=CurrentState.NextNode;
			Length+=1;
			RaiseUndoBufferChanged();
		}

		public void MoveBack()
		{
			if(!CanMoveBack)
			{
				throw new InvalidOperationException(
					"History.MoveBack() cannot execute because"
					+" CanMoveBack returned false (the current state"
					+" is the last state in the undo buffer.");
			}
			CurrentState.PreviousAction.UnExecute();
			CurrentState=CurrentState.PreviousNode;
			Length-=1;
			RaiseUndoBufferChanged();
		}

		public bool CanMoveForward
		{
			get
			{
				return CurrentState.NextAction!=null&&
					CurrentState.NextNode!=null;
			}
		}

		public bool CanMoveBack
		{
			get
			{
				return CurrentState.PreviousAction!=null&&
					CurrentState.PreviousNode!=null;
			}
		}

		/// <summary>
		/// The length of Undo buffer (total number of undoable actions).
		/// </summary>
		public int Length { get; set; }

		public IEnumerator<IAction> GetEnumerator()
		{
			return EnumUndoableActions().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
