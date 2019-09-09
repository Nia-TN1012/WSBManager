using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WSBManager.Common {
	public sealed class ThemeToImageSourceConverter : IValueConverter {
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is ApplicationTheme ? $"{parameter.ToString()}_{( ( ApplicationTheme )value ).ToString()}.png" : $"{parameter.ToString()}_{App.Current.RequestedTheme.ToString()}.png";

		public object ConvertBack( object value, Type targetType, object parameter, string language ) => null;
	}
}
