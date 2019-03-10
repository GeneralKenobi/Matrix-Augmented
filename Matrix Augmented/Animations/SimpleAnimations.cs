using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Matrix_Augmented
{
	/// <summary>
	/// This class contains methods which perform simple animations on <see cref="UIElement"/>s
	/// </summary>
	public static class SimpleAnimations
    {
		#region Slide Animation

		/// <summary>
		/// Slides the UIElement
		/// </summary>
		/// <param name="target">UIElement to slide</param>
		/// <param name="orientation">orientation of the slide</param>
		/// <param name="from">position to start from (current is null)</param>
		/// <param name="to">final position</param>
		/// <param name="duration">Duration of the slide, in miliseconds</param>
		/// <param name="startTime">Start time of the animation</param>
		/// <param name="easing">Type of easing (default is <see cref="ExponentialEase"/>)</param>
		public static async Task SlideAsync(this UIElement target, Orientation orientation, double? from, double to,
			int duration = 400, int startTime = 0, EasingFunctionBase easing = null)
		{
			// Check if the caller specified easing, if not use the default
			if (easing == null)
			{
				easing = new ExponentialEase();
			}

			// Create a new CompositeTransform
			var transform = new CompositeTransform();

			// set it to the target UIElement
			target.RenderTransform = transform;

			// Define origin
			target.RenderTransformOrigin = new Point(0.5, 0.5);

			// Create a new double animation and assign passed values
			var db = new DoubleAnimation
			{
				To = to,
				From = from,
				EasingFunction = easing,
				Duration = TimeSpan.FromMilliseconds(duration),
			};

			// Set target in the storyboard
			Storyboard.SetTarget(db, target);

			// Set the target property
			var axis = orientation == Orientation.Horizontal ? "X" : "Y";
			Storyboard.SetTargetProperty(db, $"(UIElement.RenderTransform).(CompositeTransform.Translate{axis})");

			// Create a new storyboard and assign the begin time
			var sb = new Storyboard
			{
				BeginTime = TimeSpan.FromMilliseconds(startTime)
			};

			// Add the animation to the storyboard
			sb.Children.Add(db);

			// Run the storyboard
			sb.Begin();

			// Await for it to end
			await Task.Delay(duration);
		}

		#endregion

		#region Animation on Children

		/// <summary>
		/// Slides all children in the container that pass the predicate
		/// </summary>
		/// <typeparam name="T">Type of the children element to target, must be derived from <see cref="UIElement"/></typeparam>
		/// <param name="panel">Container to go through</param>
		/// <param name="orientation">Orientation of the slide</param>
		/// <param name="from">Starting position</param>
		/// <param name="to">Ending position</param>
		/// <param name="duration">Duration of the animation, in miliseconds</param>
		/// <param name="predicate">Predicate which determines if this item is to be animated</param>
		/// <param name="easing">Easing function to use, default is <see cref="ExponentialEase"/></param>
		/// <param name="startTime">Start time of the animation, default is 0</param>
		public static async Task SlideChildren<T>(this Panel panel, Orientation orientation, double from, double to,
			int duration, Predicate<T> predicate, EasingFunctionBase easing = null, int startTime = 0)
			where T : UIElement 
		{
			// For each children in the container...
			foreach (var item in panel.Children)
			{
				// If it's of type T...
				if (item is T t)
				{
					// And it passes the predicate...
					if (predicate(t))
					{
						// Slide it
						t.SlideAsync(Orientation.Horizontal, from, to, duration, startTime);
					}
				}
			}

			// Await so that the caller can also await
			await Task.Delay(duration);
		}

		/// <summary>
		/// Slides all children in the container
		/// </summary>
		/// <typeparam name="T">Type of the children element to target, must be derived from <see cref="UIElement"/></typeparam>
		/// <param name="panel">Container to go through</param>
		/// <param name="orientation">Orientation of the slide</param>
		/// <param name="from">Starting position</param>
		/// <param name="to">Ending position</param>
		/// <param name="duration">Duration of the animation, in miliseconds</param>
		/// <param name="easing">Easing function to use, default is <see cref="ExponentialEase"/></param>
		/// <param name="startTime">Start time of the animation, default is 0</param>
		public static async Task SlideChildren<T>(this Panel panel, Orientation orientation, double from, double to,
			int duration, EasingFunctionBase easing = null, int startTime = 0)
			where T : UIElement
		{
			// For each children in the container...
			foreach (var item in panel.Children)
			{
				// If it's of type T...
				if (item is T t)
				{					
					// Slide it
					t.SlideAsync(Orientation.Horizontal, from, to, duration, startTime);
					
				}
			}

			// Await so that the caller can also await
			await Task.Delay(duration);
		}

		#endregion

		#region Opacity Animation

		/// <summary>
		/// Performs a <see cref="DoubleAnimation"/> on <see cref="UIElement"/>'s opacity
		/// </summary>
		/// <param name="element">Element to animate</param>
		/// <param name="from">Value to start at</param>
		/// <param name="to">Value to end at</param>
		/// <param name="duration">Duration of the animation</param>
		/// <param name="easing">Easing used in the animation, default is <see cref="CubicEase"/></param>
		/// <returns></returns>
		public static async Task OpacityAnimation(this UIElement element, double from, double to, int duration = 400, EasingFunctionBase easing = null)
		{
			// Create a new storyboard
			var sb = new Storyboard();

			// If the easing wasn't specified by the user, assign default
			if(easing==null)
			{
				easing = new CubicEase();
			}

			// Create a new animation and configure properties
			var animation = new DoubleAnimation()
			{
				From = from,
				To = to,
				Duration = TimeSpan.FromMilliseconds(duration),
				EasingFunction = easing,
			};

			// Set the target and target property
			Storyboard.SetTarget(animation, element);
			Storyboard.SetTargetProperty(animation, "Opacity");

			// Add the animation to the storyboard
			sb.Children.Add(animation);

			// And run it
			sb.Begin();

			// Await so that if the user wishes, he can await this animation
			await Task.Delay(duration);
		}

		#endregion

	}
}
