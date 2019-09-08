using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace WSBManager.Common {

	/// <summary>
	/// Provides the method whitch converts a value in accordance with the specified format and culture information.
	/// </summary>
	public sealed class ValueToStringFormatConverter : IValueConverter {

		/// <summary>
		///	Converts a value in accordance with the specified format and culture information.
		/// </summary>
		/// <param name="value">Source value</param>
		/// <param name="targetType">Target type</param>
		/// <param name="parameter">Format string</param>
		/// <param name="language">Culture information</param>
		/// <returns>String that has been converted by the format and culture information</returns>
		public object Convert( object value, Type targetType, object parameter, string language ) {
			// Using Format and Culture infomation
			if( parameter is string && language != null ) {
				return string.Format( new CultureInfo( language ), ( string )parameter, value );
			}
			// Using Format only
			else if( parameter is string ) {
				return string.Format( ( string )parameter, value );
			}
			// Default
			return value.ToString();
		}

		/// <summary>
		///	This method is not using. always returns null.
		/// </summary>
		public object ConvertBack( object value, Type targetType, object parameter, string language ) => null;
	}
}
