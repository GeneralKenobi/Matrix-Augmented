using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_Augmented.Core
{

	/// <summary>
	/// Class containing methods which solve equations
	/// </summary>
	public static class Solver
	{

		#region Computation

		public static CalcMatrix Compute(string input, List<CalcMatrix> matrices)
		{
			// Queue with operations and CalcMatrices in Reverse Polish
			Queue<string> output = Input.TransformIntoRP(input, matrices);

			// Stack for CalcMatrices
			Stack<CalcMatrix> stack = new Stack<CalcMatrix>();

			// Go through the whole queue
			while(output.Count != 0)
			{
				try
				{
					switch (output.Peek())
					{

						// Multiplication
						case "*":
							{
								// Calculate the result and Push it onto the stack
								stack.Push(stack.Pop() * stack.Pop());
							}
							break;

						// Addition
						case "+":
							{								
								// Calculate the result and Push it onto the stack
								stack.Push(stack.Pop() + stack.Pop());								
							}
							break;

						// Subtraction
						case "-":
							{
								CalcMatrix m1 = stack.Pop();
								CalcMatrix m2 = stack.Pop();
								// Calculate the result and Push it onto the stack
								stack.Push(m2 - m1);
							}
							break;

						// We're expecting a CalcMatrix
						default:
							{
								// Push a new CalcMatrix based on the found matrix, ID doesn't matter anymore since we take elements
								// from the top of the stack
								stack.Push(new CalcMatrix(matrices.Find((x) => x.CalcID.ToString() == output.Peek())));
							}
							break;
					}
				}
				catch(InvalidOperationException e)
				{
					throw new Exception("Incorrect input");
				}

				output.Dequeue();
			}

			// Finally, the result is the last matrix on the stack
			return stack.Pop();
		}
		
		#endregion
		
		

	}
}
