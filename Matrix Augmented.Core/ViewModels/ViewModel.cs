using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Matrix_Augmented.Core
{
	/// <summary>
	/// View model for the main page of this application
	/// </summary>
	public class ViewModel : BaseViewModel, INotifyPropertyChanged
	{
		#region Private Members

		/// <summary>
		/// Flag used to indicate if the current matrix is currently modified
		/// </summary>
		private bool TransformationFlag { get; set; } = false;

		private string mMessage;

		private Fraction mResult;

		private bool mShowDecimal;

		#endregion

		#region Private Properties

		/// <summary>
		/// The result which is shown on the bottom menu
		/// </summary>
		private Fraction Result
		{
			get => mResult;
			set
			{
				mResult = value == null ? null : new Fraction(value);
				OnPropertyChanged(nameof(ResultDecimal));
				OnPropertyChanged(nameof(ResultNumerator));
				OnPropertyChanged(nameof(ResultDenominator));
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// All matrices the user has defined
		/// </summary>
		public List<Matrix> Matrices { get; set; } = new List<Matrix>();

		/// <summary>
		/// Currently selected matrix
		/// </summary>
		public Matrix CurrentMatrix { get; set; }

		/// <summary>
		/// The message which is shown on the bottom menu
		/// </summary>
		public string Message
		{
			get => mMessage;
			set
			{
				mMessage = value;
				OnPropertyChanged(nameof(Message));
			}
		}

		/// <summary>
		/// Result's decimal representation
		/// </summary>
		public string ResultDecimal => Result?.Decimal.ToString();
		
		public string ResultNumerator => Result?.Numerator.ToString();

		public string ResultDenominator => Result?.Denominator.ToString();

		/// <summary>
		/// True if there can be more matrices added, false otherwise
		/// </summary>
		public bool CanAddMatrix => FirstAvailableID() != '\0';

		/// <summary>
		/// True if the user can delete the current matrix
		/// </summary>
		public bool CanDeleteMatrix => Matrices.Count > 1;

		/// <summary>
		/// If true, Decimal values will be shown, if false - standard fraction values
		/// </summary>
		public bool ShowDecimal
		{
			get => mShowDecimal;
			set
			{
				mShowDecimal = value;
				OnPropertyChanged(nameof(ShowDecimal));
			}
		}

		#endregion

		#region Current Matrix Resize Event

		/// <summary>
		/// Event for when the size of the currently selected Matrix has changed
		/// </summary>
		public event ResizeEventHandler CurrentMatrixResize;

		/// <summary>
		/// Method for the CurrentSize event
		/// </summary>
		/// <param name="e"></param>
		private void OnCurrentMatrixResize(ResizeEventArgs e) => CurrentMatrixResize?.Invoke(this, e);

		#endregion

		#region Matrix Added/Removed Event

		/// <summary>
		/// Event for when the size of the currently selected Matrix has changed
		/// </summary>
		public event MatricesModifiedEventHandler MatrixAddedDeleted;

		/// <summary>
		/// Method for the CurrentSize event
		/// </summary>
		/// <param name="e"></param>
		private void OnMatrixAddedDeleted(MatricesModifiedEventArgs e) => MatrixAddedDeleted?.Invoke(this, e);

		#endregion

		#region CurrentMatrixChangedEvent

		/// <summary>
		/// Event for when the current matrix has changed
		/// </summary>
		public event MatricesModifiedEventHandler CurrentMatrixModified;

		/// <summary>
		/// Method for CurrentMatrixChanged event
		/// </summary>
		/// <param name="e"></param>
		private void OnCurrentMatrixModified(MatricesModifiedEventArgs e) => CurrentMatrixModified?.Invoke(this, e);

		#endregion

		#region Creation of new matrix

		/// <summary>
		/// If possible, adds a new matrix and assigns it the first available ID
		/// </summary>
		/// <param name="array">Matrix to add</param>
		/// <returns>The ID of the created matrix</returns>
		private char AddNewMatrix(Fraction[,] array = null)
		{
			// If we can't add any more matrices, return
			if (!CanAddMatrix)
			{
				return '\0';
			}

			// Get the ID
			char id = FirstAvailableID();
			
			// If matrix wasn't initialized
			if(array == null)
			{
				array = new Fraction[1, 1];
				array.RemoveNull();
			}

			// Create the new matrix
			var matrix = new Matrix(id, array);

			// Add the delegate
			matrix.SizeChanged += OnMatrixSizeChanged;

			// Add the matrix
			Matrices.Add(matrix);

			// Notify the clients
			OnMatrixAddedDeleted(new MatricesModifiedEventArgs(id, OperationType.Addition));
			OnPropertyChanged(nameof(CanDeleteMatrix));
			OnPropertyChanged(nameof(CanAddMatrix));

			return id;
		}

		/// <summary>
		/// Returns the first available ID to use, or if none are available '\0'
		/// </summary>
		/// <returns></returns>
		private char FirstAvailableID()
		{
			// For each letter...
			for (char x = 'A'; x <= 'Z'; ++x)
			{
				// If there isn't a matrix with such ID...
				if (Matrices.TrueForAll((item) => item.ID != x))
				{
					// Return it as the first available ID
					return x;
				}
			}

			// Else return null
			return '\0';
		}

		#endregion

		#region Removal of a Matrix
		
		/// <summary>
		/// Removes the current matrix
		/// </summary>
		private void RemoveCurrentMatrix()
		{
			// If it's the last matrix, return
			if (!CanDeleteMatrix)
			{
				return;
			}

			RemoveMatrix(CurrentMatrix);
		}

		/// <summary>
		/// Removes the matrix denoted by ID
		/// </summary>
		/// <param name="id">ID of the matrix to remove</param>
		private void RemoveMatrix(Matrix matrix)
		{
			// If it's the last matrix, return
			if (!CanDeleteMatrix)
			{
				return;
			}
						
			Matrices.Remove(matrix);
		}

		/// <summary>
		/// Removes the matrix denoted by ID
		/// </summary>
		/// <param name="id">ID of the matrix to remove</param>
		private void RemoveMatrix(char id)
		{
			// If it's the last matrix, return
			if(!CanDeleteMatrix)
			{
				return;
			}

			Matrix toRemove = null;

			// Find the matrix to remove
			Matrices.ForEach((x) =>
			{
				if(x.ID==id)
				{
					toRemove = x;
				}
			});

			// Remove it
			Matrices.Remove(toRemove);
		}

		#endregion

		#region Commands

		/// <summary>
		/// Transformation Command for adding a new row to the current matrix.
		/// <para>
		/// Duration: <see cref="Constants.ShortAnimation"/>
		/// New row is added at the beginning
		/// </para>
		/// </summary>
		public ICommand AddRowCommand { get; set; }

		/// <summary>
		/// Transformation Command for adding a new column to the current matrix
		/// <para>
		/// Duration: <see cref="Constants.ShortAnimation"/>
		/// New column is added at the beginning
		/// </para>
		/// </summary>
		public ICommand AddColumnCommand { get; set; }

		/// <summary>
		/// Transformation Command for deleting a row from the current matrix
		/// <para>
		/// Duration: <see cref="Constants.ShortAnimation"/>
		/// Row is deleted at the beginning
		/// </para>
		/// </summary>
		public ICommand DeleteRowCommand { get; set; }

		/// <summary>
		/// Transformation Command for deleting a column from the current matrix
		/// <para>
		/// Duration: <see cref="Constants.ShortAnimation"/>
		/// Column is deleted at the beginning
		/// </para>
		/// </summary>
		public ICommand DeleteColumnCommand { get; set; }

		/// <summary>
		/// Command for clearing fields in this matrix
		/// </summary>
		public ICommand ClearCommand { get; set; }

		/// <summary>
		/// Command for calculating the determinant of this matrix
		/// </summary>
		public ICommand DeterminantCommand { get; set; }

		/// <summary>
		/// Command for calculating the rank of this matrix
		/// </summary>
		public ICommand RankCommand { get; set; }

		/// <summary>
		/// Command for creating a new matrix
		/// <para>
		/// Duration: <see cref="Constants.VeryShortAnimation"/>
		/// </para>
		/// </summary>
		public ICommand NewMatrixCommand { get; set; }

		/// <summary>
		/// Command for deleting the current matrix
		/// <para>
		/// Duration: 2 times <see cref="Constants.ShortAnimation"/>
		/// Time = 0: Current Matrix changed.
		/// Time = <see cref="Constants.ShortAnimation"/>: Old matrix deleted, events notified
		/// </para>
		/// </summary>
		public ICommand DeleteCurrentMatrixCommand { get; set; }

		/// <summary>
		/// Transformation Command. Sets the current matrix to the matrix denoted by the parameter (which should be of type char)
		/// <para>
		/// Duration: 2 times <see cref="Constants.ShortAnimation"/>
		/// Time = 0: Current Matrix changed.
		/// </para>
		/// </summary>
		public ICommand SetCurrentMatrixCommand { get; set; }

		/// <summary>
		/// Calculates the inverse of the current matrix
		/// </summary>
		public ICommand InverseCommand { get; set; }

		/// <summary>
		/// Calculates the traingular matrix from the current matrix
		/// </summary>
		public ICommand TriangularCommand { get; set; }

		/// <summary>
		/// Calculates the row-echelon matrix from the current matrix
		/// </summary>
		public ICommand RowEchelonCommand { get; set; }

		/// <summary>
		/// Calculates the transpose matrix from the current matrix
		/// </summary>
		public ICommand TransposeCommand { get; set; }

		/// <summary>
		/// Calculates the result of the equation given by the user
		/// </summary>
		public ICommand ComputeEquationCommand { get; set; }

		/// <summary>
		/// Toggles the <see cref="ShowDecimal"/> property
		/// </summary>
		public ICommand ToggleShowDecimalCommand { get; set; }

		/// <summary>
		/// If the Current Matrix is square, makes it an identity matrix
		/// </summary>
		public ICommand IdentityCommand { get; set; }

		/// <summary>
		/// If the Current Matrix is square, decomposes it into a lower triangular matrix an upper triangular matrix
		/// </summary>
		public ICommand LUDecompositionCommand { get; set; }

		#endregion

		#region Current Matrix Change

		/// <summary>
		/// Sets a the current matrix to a chosen matrix
		/// </summary>
		/// <param name="id">Matrix to set as current matrix</param>
		private void SetAsCurrentMatrix(char id)
		{
			// If the user wants to switch to the current matrix, return
			if (CurrentMatrix?.ID == id)
			{
				return;
			}

			// For each item in the Matrices...
			Matrices.ForEach((item) =>
			{
				// If it's the one we're looking for...
				if(item.ID==id)
				{
					// Set it as current matrix
					CurrentMatrix = item;

					// Notify the event and return
					OnCurrentMatrixModified(new MatricesModifiedEventArgs(id, OperationType.Change));
					return;
				}
			});

		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor for this view model
		/// Generates the first matrix
		/// </summary>
		public ViewModel()
		{						
			SetCommands();
		}

		#region Command configuration

		/// <summary>
		/// Configures commands, should be called by constructor
		/// </summary>
		private void SetCommands()
		{
			AddRowCommand = new RelayCommand(async () =>
				await RunCommand(() => TransformationFlag, async () => await AddRowCommandMethodAsync()));

			AddColumnCommand = new RelayCommand(async () =>
				await RunCommand(() => TransformationFlag, async () => await AddColumnCommandMethodAsync()));

			DeleteRowCommand = new RelayCommand(async () =>
				await RunCommand(() => TransformationFlag, async () => await DeleteRowCommandMethodAsync()));

			DeleteColumnCommand = new RelayCommand(async () =>
				await RunCommand(() => TransformationFlag, async () => await DeleteColumnCommandMethodAsync()));

			ClearCommand = new RelayCommand(() => ClearCommandMethod());

			DeterminantCommand = new RelayCommand(() => DeterminantCommandMethod());

			RankCommand = new RelayCommand(() => RankCommandMethod());

			NewMatrixCommand = new RelayCommand(async () =>
				await RunCommand(() => TransformationFlag, NewMatrixCommandMethodAsync));

			SetCurrentMatrixCommand = new RelayParametrizedCommand(async (parameter) =>
				await RunCommand(() => TransformationFlag, async () => await SetCurrentMatrixCommandMethodAsync(parameter)));

			DeleteCurrentMatrixCommand = new RelayCommand(async () =>
				await RunCommand(() => TransformationFlag, DeleteCurrentMatrixCommandMethodAsync));

			InverseCommand = new RelayCommand(() => InverseCommandMethod());

			TriangularCommand = new RelayCommand(() => TriangularCommandMethod());

			RowEchelonCommand = new RelayCommand(() => RowEchelonCommandMethod());

			TransposeCommand = new RelayCommand(() => TransposeCommandMethod());

			ComputeEquationCommand = new RelayParametrizedCommand((parameter) => ComputeEquationCommandMethod(parameter));

			ToggleShowDecimalCommand = new RelayCommand(() => ToggleShowDecimalCommandMethod());

			IdentityCommand = new RelayCommand(() => IdentityCommandMethod());

			LUDecompositionCommand = new RelayCommand(() => LUDecompositionCommandMethod());
		}

		#endregion

		#endregion

		#region Command Methods

		/// <summary>
		/// Method invoked by <see cref="AddRowCommand"/>
		/// </summary>
		/// <returns></returns>
		private async Task AddRowCommandMethodAsync()
		{
			if (CurrentMatrix.CanAddRow)
			{
				CurrentMatrix.AddRow();
				await Task.Delay(Constants.ShortAnimation);
			}
			else
			{
				Message = "Can't add any more rows";
				Result = null;
			}
		}
		
		/// <summary>
		/// Method invoked by <see cref="AddColumnCommand"/>
		/// </summary>
		/// <returns></returns>
		private async Task AddColumnCommandMethodAsync()
		{
			if (CurrentMatrix.CanAddColumn)
			{
				CurrentMatrix.AddColumn();
				await Task.Delay(Constants.ShortAnimation);
			}
			else
			{
				Message = "Can't add any more columns";
				Result = null;
			}
		}

		/// <summary>
		/// Method invoked by <see cref="DeleteRowCommand"/>
		/// </summary>
		/// <returns></returns>
		private async Task DeleteRowCommandMethodAsync()
		{
			if (CurrentMatrix.CanDeleteRow)
			{
				CurrentMatrix.DeleteRow();
				await Task.Delay(Constants.ShortAnimation);
			}
		}

		/// <summary>
		/// Method invoked by <see cref="DeleteColumnCommand"/>
		/// </summary>
		/// <returns></returns>
		private async Task DeleteColumnCommandMethodAsync()
		{
			if (CurrentMatrix.CanDeleteColumn)
			{
				CurrentMatrix.DeleteColumn();
				await Task.Delay(Constants.ShortAnimation);
			}
		}

		/// <summary>
		/// Method invoked by <see cref="ClearCommand"/>
		/// </summary>
		/// <returns></returns>
		private void ClearCommandMethod()
		{
			CurrentMatrix.Fields.Reset();
		}

		/// <summary>
		/// Method invoked by <see cref="ToggleShowDecimalCommand"/>
		/// </summary>
		private void ToggleShowDecimalCommandMethod()
		{
			ShowDecimal = !ShowDecimal;
		}

		/// <summary>
		/// Method invoked by <see cref="DeterminantCommand"/>
		/// </summary>
		private void DeterminantCommandMethod()
		{
			string info;

			// Calculate the Determinant
			var fr = CurrentMatrix.Determinant(out info);
			
			// And save the info
			Result = fr;			
			Message = info;
		}

		/// <summary>
		/// Method invoked by <see cref="RankCommand"/>
		/// </summary>
		private void RankCommandMethod()
		{
			string info;
			Result = new Fraction(CurrentMatrix.Rank(out info));
			Message = info;
		}

		/// <summary>
		/// Method invoked by <see cref="SetCurrentMatrixCommand"/>
		/// </summary>
		/// <returns></returns>
		private async Task SetCurrentMatrixCommandMethodAsync(object parameter)
		{
			SetAsCurrentMatrix((char)parameter);
			await Task.Delay(2 * Constants.ShortAnimation);
		}

		/// <summary>
		/// Method for the <see cref="DeleteCurrentMatrixCommand"/>
		/// </summary>
		/// <returns></returns>
		private async Task DeleteCurrentMatrixCommandMethodAsync()
		{
			// If it's the last matrix, return
			if (!CanDeleteMatrix)
			{
				return;
			}

			// Find the next Matrix we're going to show
			char nextCurrentID = FindNearestMatrix(CurrentMatrix.ID);

			Matrix toDelete = CurrentMatrix;

			// Set the new CurrentMatrix
			SetAsCurrentMatrix(nextCurrentID);

			// Wait for the old Matrix to animate out
			await Task.Delay(Constants.ShortAnimation);

			// Remove the matrix and raise an event
			RemoveMatrix(toDelete);

			// Notify clients
			OnMatrixAddedDeleted(new MatricesModifiedEventArgs(toDelete.ID, OperationType.Removal));
			OnPropertyChanged(nameof(CanDeleteMatrix));
			OnPropertyChanged(nameof(CanAddMatrix));

			// Wait for the new matrix to animate in
			await Task.Delay(Constants.ShortAnimation);
		}

		/// <summary>
		/// Method for the <see cref="InverseCommand"/>
		/// </summary>
		private void InverseCommandMethod()
		{
			// If the matrix is square
			if (CurrentMatrix.IsInvertible())
			{
				// If it's invertible, calculate the inverse
				AddNewMatrix(CurrentMatrix.Inverse());
			}
			else
			{
				Message = "This matrix is non-invertible";
				Result = null;
			}			
		}

		/// <summary>
		/// Method for the <see cref="TriangularCommand"/>
		/// </summary>
		private void TriangularCommandMethod()
		{
			// If current matrix isn't square...
			if(!CurrentMatrix.Fields.IsSquare())
			{
				// Create message for the user
				Message = "Only square matrices can be triangular";
				Result = null;

				// And return
				return;
			}

			// And add it
			AddNewMatrix(CurrentMatrix.Triangular());
		}

		/// <summary>
		/// Method for the <see cref="RowEchelonCommand"/>
		/// </summary>
		private void RowEchelonCommandMethod()
		{			
			AddNewMatrix(CurrentMatrix.RowEchelon());
		}

		/// <summary>
		/// Method for the <see cref="TransposeCommand"/>
		/// </summary>
		private void TransposeCommandMethod()
		{
			AddNewMatrix(CurrentMatrix.Transpose());
		}

		/// <summary>
		/// Method for the <see cref="NewMatrixCommand"/>
		/// </summary>
		private async Task NewMatrixCommandMethodAsync()
		{
			AddNewMatrix();
			await Task.Delay(Constants.VeryShortAnimation);
		}

		/// <summary>
		/// Method for <see cref="IdentityCommand"/>
		/// </summary>
		private void IdentityCommandMethod()
		{
			// If the current matrix isn't square, return
			if(!CurrentMatrix.Fields.IsSquare())
			{
				// Write an error message and return
				Message = "Only square matrices can be identity matrices";
				Result = null;
				return;
			}

			CurrentMatrix.Fields.Identity();
		}

		/// <summary>
		/// Method for the <see cref="ComputeEquationCommand"/>
		/// </summary>
		/// <param name="parameter"><see cref="string"/> containing the equation</param>
		private void ComputeEquationCommandMethod(object parameter)
		{
			string input = parameter as string;

			// If the user didn't write anything, return
			if(string.IsNullOrWhiteSpace(input))
			{
				return;
			}
			CalcMatrix result = null;

			try
			{
				result = Solver.Compute(input, Matrices.CopyToCalcMatrixList());
			}
			catch (Exception e)
			{
				// If the Computation failed at some point
				// Show the error message and return
				Message = e.Message;
				Result = null;
				return;
			}

			// Result is a number
			if (result.IsScalar)
			{
				Message = ((string)parameter) + " =";
				Result = result.Scalar;
			}
			// Result is a matrix
			else
			{
				AddNewMatrix(result.Fields);
			}
		}

		/// <summary>
		/// Method for the <see cref="LUDecompositionCommand"/>
		/// </summary>
		private void LUDecompositionCommandMethod()
		{
			// Otherwise perform the LU decomposition
			Fraction[,] L = null;
			Fraction[,] U = null;
			CurrentMatrix.LUDecomposition(out L, out U);

			AddNewMatrix(L);			
			AddNewMatrix(U);
		}

		#endregion

		#region Any Matrix Was Resized

		/// <summary>
		/// Method called whenever one of the matrices in this view model changes size
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMatrixSizeChanged(object sender, ResizeEventArgs e)
		{
			// If the resized matrix is the currently shown matrix...
			if(e.ID == CurrentMatrix.ID)
			{
				// Notify the CurrentResize event and pass on the information
				CurrentMatrixResize?.Invoke(sender, e);
			}
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Find the id of a matrix 'nearest' to the matrix specified by id, starting on the left
		/// Ex: For Matrices: A, B, C, D, E
		/// id = D will give C
		/// id = A will give B
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		private char FindNearestMatrix(char id)
		{
			char result = '\0';

			// Create a variable for iterating to the left (to 'A')
			char iterator = id;
			--iterator;

			// For each char between iterator and 'A'...
			for(char i=iterator; i>='A'; --i)
			{
				// For each matrix...
				Matrices.ForEach((x) =>
				{
					// If there's a matrix given by i, mark it as a result
					if(x.ID==i)
					{
						result = i;
					}
				});

				// If the result was assigned...
				if(result!='\0')
				{
					// Return it
					return result;
				}
			}

			// Not we're going to the right
			iterator = id;
			++iterator;

			// For each char between iterator and 'A'...
			for (char i = iterator; i <= 'Z'; ++i)
			{
				// For each matrix...
				Matrices.ForEach((x) =>
				{
					// If there's a matrix given by i, mark it as a result
					if (x.ID == i)
					{
						result = i;
					}
				});

				// If the result was assigned...
				if (result != '\0')
				{
					// Return it
					return result;
				}
			}

			// Return null (nothing was found)
			return '\0';
		}

		#endregion

	}

}
