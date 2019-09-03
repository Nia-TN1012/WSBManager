using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WSBManager.Common {
	public sealed class BoolToVisibilityConverter : IValueConverter {
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is bool && ( bool )value ? Visibility.Visible : Visibility.Collapsed;

		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is Visibility && ( Visibility )value == Visibility.Visible;
	}
}

