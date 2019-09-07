using System;
using Windows.UI.Xaml.Data;

using WSBManager.Models;

namespace WSBManager.Common {
	public sealed class VGpuToListIndexConverter : IValueConverter {
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is VGpu ? ( int )value : ( int )VGpu.Default;

		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is int ? ( VGpu )value : VGpu.Default;
	}

	public sealed class NetworkToListIndexConverter : IValueConverter {
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is Networking ? ( int )value : ( int )Networking.Default;

		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is int ? ( Networking )value : Networking.Default;
	}
}
