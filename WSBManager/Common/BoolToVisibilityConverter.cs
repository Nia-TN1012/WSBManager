using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WSBManager.Common
{
	/// <summary>
	/// Provides the method that converts a <see cref="bool"/> value to a <see cref="Visibility"/> value.
	/// </summary>
	public sealed class BoolToVisibilityConverter : IValueConverter
	{
		/// <summary>
		/// Converts a <see cref="bool"/> value to a <see cref="Visibility"/> value.
		/// </summary>
		/// <param name="value"><see cref="bool"/> value</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns><see cref="Visibility"/> value</returns>
		public object Convert(object value, Type targetType, object parameter, string language) =>
			value is bool && (bool)value ? Visibility.Visible : Visibility.Collapsed;

		/// <summary>
		/// Converts a <see cref="Visibility"/> value to a <see cref="bool"/> value.
		/// </summary>
		/// <param name="value"><see cref="Visibility"/> value</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns>Bool value</returns>
		public object ConvertBack(object value, Type targetType, object parameter, string language) =>
			value is Visibility && (Visibility)value == Visibility.Visible;
	}
}

