using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace WSBManager.Common
{
	/// <summary>
	/// Provides the method that converts a <see cref="bool"/> value to a text value (Enabled / Disabled).
	/// </summary>
	public class BoolToEnabledTextConverter : IValueConverter
	{
		/// <summary>
		/// Converts a <see cref="bool"/> value to a text value (Enabled / Disabled).
		/// </summary>
		/// <param name="value"><see cref="bool"/> value</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns>text value</returns>
		public object Convert(object value, Type targetType, object parameter, string language) {
			var resourceLoader = ResourceLoader.GetForCurrentView();
			if (resourceLoader == null)
			{
				return null;
			}
			return value is bool && (bool)value ? resourceLoader.GetString("Enabled") : resourceLoader.GetString("Disabled");
		}

		/// <summary>
		/// This method is not using. always returns null.
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, string language) => null;
	}
}
