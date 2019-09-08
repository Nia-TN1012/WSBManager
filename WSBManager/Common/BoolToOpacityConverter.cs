using System;
using Windows.UI.Xaml.Data;

namespace WSBManager.Common {

	/// <summary>
	/// Provides the method that converts a bool value to a opacity.
	/// </summary>
	public sealed class BoolToOpacityConverter : IValueConverter {

		/// <summary>
		/// High value
		/// </summary>
		private const double high = 1.0;
		/// <summary>
		/// Low value
		/// </summary>
		private const double low = 0.3;

		/// <summary>
		/// Converts a bool value to a opacity.
		/// </summary>
		/// <param name="value">Bool value</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns>Opacity value</returns>
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is bool && ( bool )value ? high : low;

		/// <summary>
		/// This method is not using. always returns null.
		/// </summary>
		public object ConvertBack( object value, Type targetType, object parameter, string language ) => null;
	}
}
