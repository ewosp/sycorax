using System;
using System.Collections.Generic;
using System.Text;

namespace Sycorax {
    /// <summary>
    /// MessageEventArgs
    /// </summary>
    public class MessageEventArgs : EventArgs {
        public MessageEventArgs (string message) {
            this.message = message;
        }

        private string message;

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message {
            get { return message; }
        }
    }
}
