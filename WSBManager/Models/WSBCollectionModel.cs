using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Storage;
using System.Runtime.CompilerServices;

namespace WSBManager {

	/// <summary>
	/// The list of Windows Sandbox configuration model.
	/// </summary>
	class WSBCollectionModel : INotifyPropertyChanged {

		/// <summary>
		/// The list of Windows Sandbox configurations.
		/// </summary>
		public Dictionary<string, WSBModel> WSBCollection { get; private set; }

		/// <summary>
		///	The event handler to be generated after the property changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
