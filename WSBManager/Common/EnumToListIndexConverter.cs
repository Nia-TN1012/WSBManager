using System;
using Windows.UI.Xaml.Data;

using WSBManager.Configurations;

namespace WSBManager.Common {
	public sealed class VGpuToListIndexConverter : IValueConverter {
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is VGpu ? ( int )value : ( int )VGpu.Default;

		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is int ? ( VGpu )value : VGpu.Default;
	}

	public sealed class NetworkToListIndexConverter : IValueConverter {
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is Network ? ( int )value : ( int )Network.Default;

		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is int ? ( Network )value : Network.Default;
	}
}
