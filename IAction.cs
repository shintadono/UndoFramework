namespace UndoFramework
{
	/// <summary>
	/// Encapsulates a user action (actually two actions: Do and Undo).
	/// Can be anything.
	/// You can give your implementation any information it needs to be able to
	/// execute and rollback what it needs.
	/// </summary>
	public interface IAction
	{
		/// <summary>
		/// Name of Action
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Apply changes encapsulated by this object.
		/// </summary>
		/// <remarks>
		/// ExecuteCount++
		/// </remarks>
		void Execute();

		/// <summary>
		/// Undo changes made by a previous Execute call.
		/// </summary>
		/// <remarks>
		/// ExecuteCount--
		/// </remarks>
		void UnExecute();

		/// <summary>
		/// For most Actions, CanExecute is <b>true</b> when ExecuteCount = 0 (not yet executed)
		/// and <b>false</b> when ExecuteCount = 1 (already executed once).
		/// </summary>
		/// <returns><b>true</b> if an encapsulated action can be applied.</returns>
		bool CanExecute();

		/// <summary>For most Actions, CanUnExecute = !CanExecute.</summary>
		/// <returns><b>true</b> if an action was already executed and can be undone.</returns>
		bool CanUnExecute();

		/// <summary>
		/// Attempts to take a new incoming action and instead of recording that one
		/// as a new action, just modify the current one so that it's summary effect is
		/// a combination of both.
		/// </summary>
		/// <param name="followingAction">The incoming action.</param>
		/// <returns><b>true</b> if the action agreed to merge, <b>false</b> if we want the followingAction
		/// to be tracked separately.</returns>
		bool TryToMerge(IAction followingAction);

		/// <summary>
		/// Defines if the action can be merged with the previous one in the Undo buffer.
		/// This is useful for long chains of consecutive operations of the same type,
		/// e.g. dragging something or typing some text.
		/// </summary>
		bool AllowToMergeWithPrevious { get; set; }
	}
}
