using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WSBManager.Common
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class SelectedIndexToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language) =>
			value is int && (int)value > -1 ? Visibility.Visible : Visibility.Collapsed;

		public object ConvertBack(object value, Type targetType, object parameter, string language) =>
			throw new NotImplementedException();
	}
}
