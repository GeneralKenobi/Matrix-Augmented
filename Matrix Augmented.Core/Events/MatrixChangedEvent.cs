using System;

namespace Matrix_Augmented.Core
{

	/// <summary>
	/// Event for when a matrix is resized
	/// </summary>
	/// <param name="sender">Matrix that is resized</param>
	/// <param name="e">Information about the resize</param>
	public delegate void MatricesModifiedEventHandler(object sender, MatricesModifiedEventArgs e);


	/// <summary>
	/// Enum denoting the type of the operation (addition or removal)
	/// </summary>
	public enum OperationType
	{
		/// <summary>
		/// A new matrix was added
		/// </summary>
		Addition = 0,

		/// <summary>
		/// A matrix was removed
		/// </summary>
		Removal = 1,

		/// <summary>
		/// A matrix was changed
		/// </summary>
		Change = 2,
	}

	/// <summary>
	/// Class for argument of the resize event
	/// </summary>
	public class MatricesModifiedEventArgs : EventArgs
	{

		#region Public Properties

		/// <summary>
		/// ID of the added/deleted matrix
		/// </summary>
		public char ID { get; private set; }
		
		/// <summary>
		/// Type of the resize
		/// </summary>
		public OperationType Type { get; private set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor for the argument of the resize event
		/// </summary>
		/// <param name="id">ID of the added/deleted matrix</param>
		/// <param name="type">Type of the operation</param>
		public MatricesModifiedEventArgs(char id, OperationType type)
		{
			// Assign the information
			ID = id;
			Type = type;
		}

		#endregion
	}
}
