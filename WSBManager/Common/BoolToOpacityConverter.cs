using System;
using Windows.UI.Xaml.Data;

namespace WSBManager.Common {

	public sealed class BoolToOpacityConverter : IValueConverter {

		private const double threshold = 0.3;

		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is bool && ( bool )value ? 1.0 : threshold;

		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is double && ( double )value > threshold;
	}
}
