
namespace Matrix_Augmented
{

	/// <summary>
	/// Type of slide in animation to perform on a FractionBox
	/// </summary>
	public enum SlideInAnimationType
	{
		/// <summary>
		/// Slides in from bottom, from beneath the page
		/// </summary>
		FromBottom = 1,

		/// <summary>
		/// Slides in from right, from beyond the page
		/// </summary>
		FromRight = 2,

		/// <summary>
		/// Slides in from the exact position of the textbox over it
		/// </summary>
		FromUpperFractionBox = 3,

		/// <summary>
		/// Slides in from the exact position of the textbox to the left
		/// </summary>
		FromLeftFractionBox = 4,

		/// <summary>
		/// No animation is performed
		/// </summary>
		None = 5,
	}

	/// <summary>
	/// Type of slide out animation to perform on a FractionBox
	/// </summary>
	public enum SlideOutAnimationType
	{
		/// <summary>
		/// Slides out to the bottom, beneath the page
		/// </summary>
		ToBottom = 1,

		/// <summary>
		/// Slides out to the right, beyond the page
		/// </summary>
		ToRight = 2,

		/// <summary>
		/// Slides out to the exact position of the textbox over it
		/// </summary>
		ToUpperFractionBox = 3,

		/// <summary>
		/// Slides out to the exact position of the textbox on the left
		/// </summary>
		ToLeftFractionBox = 4,

		/// <summary>
		/// No animation is performed
		/// </summary>
		None = 5,
	}

}
