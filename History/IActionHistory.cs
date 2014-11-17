using System;
using System.Collections.Generic;

namespace UndoFramework
{
	/// <summary>
	/// A notion of the buffer. Instead of two stacks, it's a state machine
	/// with the current state. It can move one state back or one state forward.
	/// Allows for non-linear buffers, where you can choose one of several actions to redo.
	/// </summary>
	internal interface IActionHistory : IEnumerable<IAction>
	{
		/// <summary>
		/// Appends an action to the end of the Undo buffer.
		/// </summary>
		/// <param name="newAction">An action to append.</param>
		/// <returns><b>false</b> if merged with previous, otherwise <b>true</b>.</returns>
		bool AppendAction(IAction newAction);
		void Clear();

		void MoveBack();
		void MoveForward();

		bool CanMoveBack { get; }
		bool CanMoveForward { get; }
		int Length { get; }

		SimpleHistoryNode CurrentState { get; }

		IEnumerable<IAction> EnumUndoableActions();

		IEnumerable<IAction> RedoableActions { get; }
		IEnumerable<IAction> UndoableActions { get; }

		event EventHandler CollectionChanged;
	}
}
