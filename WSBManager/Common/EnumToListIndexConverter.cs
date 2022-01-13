using System;
using Windows.UI.Xaml.Data;

using WSBManager.Models;

namespace WSBManager.Common
{
	/// <summary>
	/// Provides the method that converts <see cref="ThreeState"/> value to a selected index in list.
	/// </summary>
	public sealed class ThreeStateToListIndexConverter : IValueConverter
	{
		/// <summary>
		/// Converts <see cref="ThreeState"/> value to a selected index in list.
		/// </summary>
		/// <param name="value"><see cref="ThreeState"/> value</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language) =>
			value is ThreeState ? (int)value : (int)ThreeState.Default;

		/// <summary>
		/// Converts a selected index in list to <see cref="ThreeState"/> value.
		/// </summary>
		/// <param name="value">Selected index</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns><see cref="ThreeState"/> value</returns>
		public object ConvertBack(object value, Type targetType, object parameter, string language) =>
			value is int ? (ThreeState)value : ThreeState.Default;
	}

	/// <summary>
	/// Provides the method that converts <see cref="TwoState"/> value to a selected index in list.
	/// </summary>
	public sealed class TwoStateToListIndexConverter : IValueConverter
	{
		/// <summary>
		/// Converts <see cref="TwoState"/> value to a selected index in list.
		/// </summary>
		/// <param name="value"><see cref="TwoState"/> value</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns></returns>
		public object Convert(object value, Type targetType, object parameter, string language) =>
			value is TwoState ? (int)value : (int)TwoState.Default;

		/// <summary>
		/// Converts a selected index in list to <see cref="TwoState"/> value.
		/// </summary>
		/// <param name="value">Selected index</param>
		/// <param name="targetType">Target type ( Not using )</param>
		/// <param name="parameter">Parameter ( Not using )</param>
		/// <param name="language">Language ( Not using )</param>
		/// <returns><see cref="TwoState"/> value</returns>
		public object ConvertBack(object value, Type targetType, object parameter, string language) =>
			value is int ? (TwoState)value : TwoState.Default;
	}
}
