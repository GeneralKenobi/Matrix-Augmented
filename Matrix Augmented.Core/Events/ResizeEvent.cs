using System;

namespace Matrix_Augmented.Core
{

	/// <summary>
	/// Event for when a matrix is resized
	/// </summary>
	/// <param name="sender">Matrix that is resized</param>
	/// <param name="e">Information about the resize</param>
	public delegate void ResizeEventHandler(object sender, ResizeEventArgs e);


	/// <summary>
	/// Enum denoting the type of the resize
	/// </summary>
	public enum ResizeType
	{
		/// <summary>
		/// A new row was added
		/// </summary>
		NewRow = 0,

		/// <summary>
		/// A new column was added
		/// </summary>
		NewColumn = 1,

		/// <summary>
		/// The outer row was removed
		/// </summary>
		RemovedRow = 2,

		/// <summary>
		/// The outer column was removed
		/// </summary>
		RemovedColumn = 3,

		/// <summary>
		/// The resize is composed of more operations
		/// </summary>
		Complex = 4,
	}

	/// <summary>
	/// Class for argument of the resize event
	/// </summary>
	public class ResizeEventArgs : EventArgs
	{

		#region Public Properties

		/// <summary>
		/// ID of the resized matrix
		/// </summary>
		public char ID { get; private set; }

		/// <summary>
		/// Number of rows before the resize
		/// </summary>
		public int OldRows { get; private set; }

		/// <summary>
		/// Number of columns before the resize
		/// </summary>
		public int OldColumns { get; private set; }

		/// <summary>
		/// Number of rows after the resize (currently)
		/// </summary>
		public int NewRows { get; private set; }

		/// <summary>
		/// Number of columns after the resize (currently)
		/// </summary>
		public int NewColumns { get; private set; }

		/// <summary>
		/// Type of the resize
		/// </summary>
		public ResizeType Type { get; private set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor for the argument of the resize event
		/// </summary>
		/// <param name="oldRows">Number of rows before the resize</param>
		/// <param name="oldColumns">Number of columns before the resize</param>
		/// <param name="newRows">Number of rows after the resize (currently)</param>
		/// <param name="newColumns">Number of columns after the resize (currently)</param>
		public ResizeEventArgs(char id, int oldRows, int oldColumns, int newRows, int newColumns)
		{
			// Assign the information
			ID = id;
			OldRows = oldRows;
			OldColumns = oldColumns;
			NewRows = newRows;
			NewColumns = newColumns;

			// Determine the type of the resize:
			if (newRows - oldRows == 1 && oldColumns == newColumns)
			{
				// Row added
				Type = ResizeType.NewRow;
			}
			else
				if (newRows == oldRows && newColumns - oldColumns == 1)
			{
				// Column added
				Type = ResizeType.NewColumn;
			}
			else
				if (oldRows - newRows == 1 && oldColumns == newColumns)
			{
				// Row removed
				Type = ResizeType.RemovedRow;
			}
			else
				if (newRows == oldRows && oldColumns - newColumns == 1)
			{
				// Column removed
				Type = ResizeType.RemovedColumn;
			}
			else
			{
				// Complex resize
				Type = ResizeType.Complex;
			}

		}

		#endregion
	}
}
