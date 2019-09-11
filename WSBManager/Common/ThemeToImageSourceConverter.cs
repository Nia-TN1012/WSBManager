using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WSBManager.Common {

	/// <summary>
	/// Provides the method that converts a <see cref="ApplicationTheme"/> value to an image source value.
	/// </summary>
	public sealed class ThemeToImageSourceConverter : IValueConverter {

		/// <summary>
		/// Converts a <see cref="ApplicationTheme"/> value to an image source value.
		/// </summary>
		/// <param name="value"><see cref="ApplicationTheme"/> value (if null, Specifies the current requested theme)</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns>Image source value</returns>
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is ApplicationTheme ? $"{parameter.ToString()}_{( ( ApplicationTheme )value ).ToString()}.png" : $"{parameter.ToString()}_{App.Current.RequestedTheme.ToString()}.png";

		/// <summary>
		/// This method is not using. always returns null.
		/// </summary>
		public object ConvertBack( object value, Type targetType, object parameter, string language ) => null;
	}
}
