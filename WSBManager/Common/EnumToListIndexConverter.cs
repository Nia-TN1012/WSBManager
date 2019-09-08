using System;
using Windows.UI.Xaml.Data;

using WSBManager.Models;

namespace WSBManager.Common {

	/// <summary>
	/// Provides the method that converts <see cref="VGpu"/> value to a selected index in list.
	/// </summary>
	public sealed class VGpuToListIndexConverter : IValueConverter {

		/// <summary>
		/// Converts <see cref="VGpu"/> value to a selected index in list.
		/// </summary>
		/// <param name="value"><see cref="VGpu"/> value</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns></returns>
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is VGpu ? ( int )value : ( int )VGpu.Default;

		/// <summary>
		/// Converts a selected index in list to <see cref="VGpu"/> value.
		/// </summary>
		/// <param name="value">Selected index</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns><see cref="VGpu"/> value</returns>
		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is int ? ( VGpu )value : VGpu.Default;
	}

	/// <summary>
	/// Provides the method that converts <see cref="Networking"/> value to a selected index in list.
	/// </summary>
	public sealed class NetworkToListIndexConverter : IValueConverter {

		/// <summary>
		/// Converts <see cref="Networking"/> value to a selected index in list.
		/// </summary>
		/// <param name="value"><see cref="Networking"/> value</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns></returns>
		public object Convert( object value, Type targetType, object parameter, string language ) =>
			value is Networking ? ( int )value : ( int )Networking.Default;

		/// <summary>
		/// Converts a selected index in list to <see cref="Networking"/> value.
		/// </summary>
		/// <param name="value">Selected index</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns><see cref="Networking"/> value</returns>
		public object ConvertBack( object value, Type targetType, object parameter, string language ) =>
			value is int ? ( Networking )value : Networking.Default;
	}
}
