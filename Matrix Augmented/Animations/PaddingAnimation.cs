using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Matrix_Augmented
{
	public static class PaddingAnimation
	{

		/// <summary>
		/// Animates the element's content using bottom padding
		/// Primary and only use is for MainGrid's content
		/// </summary>
		/// <param name="element">Element to animate</param>
		/// <param name="duration">The time the animation will take in miliseconds</param>
		/// <param name="finalValue">The finalValue of the padding the element will have</param>
		/// <param name="currentValue">The current value of the element's padding</param>
		public static void Animate(this FrameworkElement element, Thickness currentValue, Thickness finalValue, int duration = 500)
		{
			var storyboard = new Storyboard();

			var slideAnimation = new ObjectAnimationUsingKeyFrames();

			// It will be roughly 60fps
			for (int i = 0; i <= duration/17; ++i)
			{
				double scalar = (double)i / duration * 17;
				slideAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame
				{
					Value = new Thickness(
					(1 - scalar) * (currentValue.Left - finalValue.Left),
					(1 - scalar) * (currentValue.Top - finalValue.Top),
					(1 - scalar) * (currentValue.Right - finalValue.Right),
					(1 - scalar) * (currentValue.Bottom - finalValue.Bottom)),
					KeyTime = TimeSpan.FromMilliseconds(scalar * duration),
				});
			}

			slideAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame
			{
				Value = finalValue,
				KeyTime = TimeSpan.FromMilliseconds(duration),
			});

			// Set the animation duration
			slideAnimation.Duration = TimeSpan.FromMilliseconds(duration);

			// Set the target and target property
			Storyboard.SetTarget(slideAnimation, element);
			Storyboard.SetTargetProperty(slideAnimation, "(FrameworkElement.Padding)");

			// Add the animation to the storyboard
			storyboard.Children.Add(slideAnimation);

			// Begin the animation
			storyboard.Begin();
		}

	}
}
