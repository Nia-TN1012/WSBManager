using System;

namespace WSBManager.Common
{
	/// <summary>
	/// Utility
	/// </summary>
	public static class Utility
	{
		/// <summary>
		/// Tries to convert <see cref="object"/> to type specified by <see cref="TypeCode"/>.
		/// </summary>
		/// <param name="value">An object that implements the <see cref="IConvertible"/> interface.</param>
		/// <param name="typeCode">The type of object to return</param>
		/// <returns>
		///		<para>An object whose underlying type is <paramref name="typeCode"/> and whose value is equivalent to <paramref name="value"/>.</para>
		///		<para>Or null if failed to convert.</para>
		/// </returns>
		public static object TryConvert(object value, TypeCode typeCode)
		{
			try
			{
				return Convert.ChangeType(value, typeCode);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Tries to convert <see cref="string"/> value to <see cref="Enum"/> value specified by <see cref="Type"/>.
		/// </summary>
		/// <param name="enumType">An enumeration type</param>
		/// <param name="value">A <see cref="string"/> value</param>
		/// <returns>
		///		<para>An object of type <paramref name="enumType"/> whose value is represented by <paramref name="value"/>.</para>
		///		<para>Or null if failed to convert.</para>
		/// </returns>
		public static object TryConvert(Type enumType, string value)
		{
			try
			{
				return Enum.Parse(enumType, value);
			}
			catch (Exception)
			{
				return null;
			}
		}

	}
}
