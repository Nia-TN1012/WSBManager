using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSBManager.Common {
	public static class Utility {

		public static object TryConvert( object value, TypeCode typeCode ) {
			try {
				return Convert.ChangeType( value, typeCode );
			}
			catch ( Exception ) {
				return null;
			}
		}

		public static object TryConvert( Type enumType, string value ) {
			try {
				return Enum.Parse( enumType, value );
			}
			catch( Exception ) {
				return null;
			}
		}

	}
}
