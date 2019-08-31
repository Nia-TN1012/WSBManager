using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSBManager.Common {
	/// <summary>
	///	Provides the event argument with the confirmation message and the call back methods.
	/// </summary>
	public class ComfirmmationActionRunEventArgs : EventArgs {

		/// <summary>
		///	Confirmation message.
		/// </summary>
		public string Message { get; private set; }

		/// <summary>
		///	Callback methods.
		/// </summary>
		public Action Callback { get; private set; }

		/// <summary>
		///	Creates a new instance of the <see cref="ComfirmmationActionRunEventArgs"/> class from the confirmation message and the callback methods.
		/// </summary>
		/// <param name="message">The confirmation message</param>
		/// <param name="callback">The call back methods</param>
		public ComfirmmationActionRunEventArgs( string message, Action callback ) {
			Message = message;
			Callback = callback;
		}
	}

	/// <summary>
	///	A method that handles the <see cref="IComfirmationActionRun.ComfirmationActionRun"/> event.
	/// </summary>
	public delegate void ComfirmationActionRunEventHandler( object sender, ComfirmmationActionRunEventArgs e );

	/// <summary>
	///	Notifies clients that a confirmation action has run.
	/// </summary>
	public interface IComfirmationActionRun {

		/// <summary>
		///	Occurs when a confirmation action has run.
		/// </summary>
		event ComfirmationActionRunEventHandler ComfirmationActionRun;
	}
}
