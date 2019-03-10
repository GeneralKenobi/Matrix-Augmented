using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented.Core
{
	/// <summary>
	/// Class used to interpret the user's input
	/// </summary>
	public static class Input
	{

		#region Reverse Polish

		/// <summary>
		/// Interprets the input and returns a queue of actions
		/// (each action is denoted by a char) in Reverse Polish Notation.
		/// If the input was invalid, it will throw an exception with user-friendly message describing the problem
		/// </summary>
		/// <param name="input">Input string to interpret</param>
		/// <param name="matrices">List of <see cref="CalcMatrix"/> that stores values later used in calculations</param>
		/// <returns>A queue of actions (each action is denoted by a char) in Reverse Polish Notation</returns>
		public static Queue<string> TransformIntoRP(string input, List<CalcMatrix> matrices)
		{
			// Transform the input into our char convention
			input = input.MakeStandard();

			// Operations stack
			Stack<char> stack = new Stack<char>();

			// Output queue
			Queue<string> output = new Queue<string>();

			// For each character in input
			for (int i = 0; i < input.Length; ++i)
			{

				switch (input[i])
				{

					// Multiplication
					case '*':
						{
							Multiplication(input, stack, output, i);
						}
						break;

					// Addition and subtraction
					case '+':
					case '-':
						{
							AdditionSubtraction(input, stack, output, i);
						}
						break;

					// Real numbers (scalars)
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						{
							Scalar(input, stack, output, i, matrices);
						}
						break;

					// Opening bracket
					case '(':
						{
							// just push it on the stack (it's the highest priority operation)
							stack.Push(input[i]);
						}
						break;

					// Closing bracket
					case ')':
						{
							CloseBracket(stack, output);
						}
						break;

					// Inverse
					case 'i':
						{
							MatrixOperation(MatrixOperationType.Inverse, input, output, ref i, matrices);
						}
						break;

					// Transpose
					case 't':
						{
							MatrixOperation(MatrixOperationType.Transposition, input, output, ref i, matrices);
						}
						break;

					// Determinant
					case 'd':
						{
							MatrixOperation(MatrixOperationType.Determinant, input, output, ref i, matrices);
						}
						break;
					
					// Rank
					case 'r':
						{
							MatrixOperation(MatrixOperationType.Rank, input, output, ref i, matrices);
						}
						break;

					default:
						{
							// Now we expect only IDs of matrices
							if (input[i] < 'A' || input[i] > 'Z')
							{
								throw new Exception($"Unknown symbol: {input[i]}");
							}

							// If the user inputted a matrix which is not defined
							if(matrices.TrueForAll((x) => x.ID != input[i]))
							{
								throw new Exception($"Matrix {input[i]} isn't defined");
							}
																				
							// We find it in the matrices list and enqueue the CalcID
							output.Enqueue(matrices.Find((x) => x.ID == input[i]).CalcID.ToString());
						}
						break;
				}
			}

			// Finally, enqueue whatever is left on the stack
			while (stack.Count != 0)
			{
				output.Enqueue(stack.Pop().ToString());
			}

			return output;
		}

		#region Input Switch Cases

		/// <summary>
		/// Calculates the inside of a bracket used with operation, for example when we have:
		/// i(A+B*C) or t(A*B*i(C))
		/// </summary>
		/// <param name="input">Input string being analized</param>
		/// <param name="output">Queue for the Reverse Polish</param>
		/// <param name="index">Current location in the input string</param>
		/// <param name="matrices">List with matrices referenced in the input</param>
		/// <param name="type">Type of the operation to perform</param>
		private static void MatrixOperation(MatrixOperationType type, string input,
			Queue<string> output, ref int index, List<CalcMatrix> matrices)
		{
			// Calculate the bracket
			CalcMatrix resultOfBracket = SubOperationBracket(input, output, ref index, matrices);

			if(resultOfBracket.IsScalar)
			{
				char typeName = '\0';

				switch (type)
				{
					case MatrixOperationType.Determinant:
						{
							typeName = 'd';
						}
						break;

					case MatrixOperationType.Rank:
						{
							typeName = 'r';
						}
						break;

					case MatrixOperationType.Transposition:
						{
							typeName = 't';
						}
						break;

					case MatrixOperationType.Inverse:
						{
							typeName = 'i';
						}
						break;
				}

				throw new Exception($"Can't calculate {typeName.ToString()}(number)");
			}

			// Get a new id
			int calcID = matrices.GetFirstFreeCalcID();

			// Declare a CalcMatrix for result
			CalcMatrix result = null;

			// Depending on the type, calculate and create result
			switch(type)
			{
				case MatrixOperationType.Transposition:
					{
						result = new CalcMatrix(calcID, resultOfBracket.Fields.Transpose());
					}
					break;

				case MatrixOperationType.Inverse:
					{
						result = new CalcMatrix(calcID, resultOfBracket.Fields.Inverse());
					}
					break;

				case MatrixOperationType.Determinant:
					{
						result = new CalcMatrix(calcID, resultOfBracket.Determinant());
					}
					break;

				case MatrixOperationType.Rank:
					{
						result = new CalcMatrix(calcID, new Fraction(resultOfBracket.Rank()));
					}
					break;
			}

			// Add it to matrices
			matrices.Add(result);

			// And enqueue it
			output.Enqueue(calcID.ToString());
		}		

		/// <summary>
		/// Calculates the inside of a bracket, used for example when we have:
		/// i(A+B*C) or t(A*B*i(C))
		/// </summary>
		/// <param name="input">Input string being analized</param>
		/// <param name="output">Queue for the Reverse Polish</param>
		/// <param name="index">Current location in the input string</param>
		/// <param name="matrices">List with matrices referenced in the input</param>
		/// <returns>The result of that bracket</returns>
		private static CalcMatrix SubOperationBracket(string input, Queue<string> output, ref int index, List<CalcMatrix> matrices)
		{
			if (input.Length > index + 1 && input[index + 1] != '(')
			{
				throw new Exception($"Incorrect input: \"{input.Substring(index, 2)}\"");
			}

			// Move index by 2, since inverse is denoted by i(A)
			index += 2;

			int beginIndex = index;

			// int tracking how many open brackets we encountered here
			int bracketDelay = 0;

			while (true)
			{
				// If the input has ended, it means it was incorrect
				if (index == input.Length)
				{
					throw new Exception("Input is incorrect. Hint: Are you missing ')' ?");
				}

				// If we opened a bracket, note it
				if (input[index] == '(')
				{
					++bracketDelay;
				}

				// If we found the closing bracket...
				if (input[index] == ')')
				{
					// If it was the original bracket in i(A), break the loop
					if (bracketDelay == 0)
					{
						break;
					}
					else
					{
						// Else just note we're looking for one closing bracket less
						--bracketDelay;
					}
				}

				++index;
			}

			return Solver.Compute(input.Substring(beginIndex, index - beginIndex), matrices);
		}

		/// <summary>
		/// Case for multiplication in the input
		/// </summary>
		/// <param name="input">Input string being analized</param>
		/// <param name="stack">Stack for the Reverse Polish</param>
		/// <param name="output">Queue for the Reverse Polish</param>
		/// <param name="index">Current location in the input string</param>
		private static void Multiplication(string input, Stack<char> stack, Queue<string> output, int index)
		{
			// As long as there are operations of higher or equal priority...
			while (stack.Count != 0 && (stack.Peek() == '*' || stack.Peek() == '/' || stack.Peek() == '^'))
			{
				// Take them off the stack and enqueue them on the output queue
				output.Enqueue(stack.Pop().ToString());
			}

			// Finally push the currently analized char onto the stack
			stack.Push(input[index]);
		}

		/// <summary>
		/// Case for Addition/Subtraction in the input
		/// </summary>
		/// <param name="input">Input string being analized</param>
		/// <param name="stack">Stack for the Reverse Polish</param>
		/// <param name="output">Queue for the Reverse Polish</param>
		/// <param name="index">Current location in the input string</param>
		private static void AdditionSubtraction(string input, Stack<char> stack, Queue<string> output, int index)
		{
			// As long as there are operations of higher or equal priority...
			while (stack.Count != 0 && (stack.Peek() == '+' || stack.Peek() == '-' ||
				stack.Peek() == '*' || stack.Peek() == '/' || stack.Peek() == '^'))
			{
				// Take them off the stack and enqueue them on the output queue
				output.Enqueue(stack.Pop().ToString());
			}

			// Finally push the currently analized char onto the stack
			stack.Push(input[index]);
		}

		/// <summary>
		/// Case for Addition/Subtraction in the input
		/// </summary>
		/// <param name="input">Input string being analized</param>
		/// <param name="stack">Stack for the Reverse Polish</param>
		/// <param name="output">Queue for the Reverse Polish</param>
		/// <param name="index">Current location in the input string</param>
		/// <param name="matrices">List with matrices referenced in the input</param>
		private static void Scalar(string input, Stack<char> stack, Queue<string> output, int index, List<CalcMatrix> matrices)
		{
			// Mark the beginning position
			int beginIndex = index;

			// Keep goind until the end of the number is found
			while (index < input.Length && ((input[index] >= '0' && input[index] <= '9') || input[index] == '/' || input[index] == '.'))
			{
				++index;
			}

			// Make a substring out of the found number and convert it to a fraction
			Fraction number = (input.Substring(beginIndex, index - beginIndex).ToFraction());

			int calcID = matrices.GetFirstFreeCalcID();

			// Add that scalar to the list
			matrices.Add(new CalcMatrix(calcID, number));

			// And enqueue it on the queue
			output.Enqueue(calcID.ToString());
		}

		/// <summary>
		/// Case for close bracket in the input
		/// </summary>
		/// <param name="stack">Stack for the Reverse Polish</param>
		/// <param name="output">Queue for the Reverse Polish</param>
		private static void CloseBracket(Stack<char> stack, Queue<string> output)
		{
			// Go until we find the opening bracket...
			while (stack.Peek() != '(')
			{
				// And enqueue every element we pop to output
				output.Enqueue(stack.Pop().ToString());

				// If for some reason we emptied the stack (error)
				if (stack.Count == 0)
				{
					throw new Exception("Input is incorrect. Hint: are you missing ')' ?");
				}
			}

			// Now that topmost item on the stack is '(', remove it
			stack.Pop();
		}


		#endregion

		#endregion

		#region Private Helpers

		/// <summary>
		/// Transforms the given string into a <see cref="Fraction"/>.
		/// If the string wasn't proper, it will throw an exception
		/// </summary>
		/// <param name="number">string to transform</param>
		/// <returns></returns>
		private static Fraction ToFraction(this string number)
		{
			// If the user used '/'
			if (number.Contains('/'))
			{
				// Create an array for numerator and denominator
				string[] parts = null;

				// Split the number string
				parts = number.Split('/');

				// And convert them into a fraction
				return new Fraction(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
			}

			// If the user used '.'
			if (number.Contains('.'))
			{
				return new Fraction(Convert.ToDouble(number));
			}

			// The user inputted an integer
			else
			{
				return new Fraction(Convert.ToInt32(number));
			}
		}		

		#endregion

		#region Input Standarization

		/// <summary>
		/// Transforms the input into a standard form
		/// <para/>
		/// Replaces ',' with '.' ; '\' with '/'
		/// </summary>
		/// <param name="input"><see cref="string"/> to standardize</param>
		/// <returns></returns>
		private static string MakeStandard(this string input)
		{
			string result = input;

			result = result.Replace(" ", string.Empty);

			// Replace commas with dots
			result = result.Replace(',', '.');

			// Backslashes with forward slashes
			result = result.Replace('\\', '/');			

			return result;
		}

		#endregion

	}
}
