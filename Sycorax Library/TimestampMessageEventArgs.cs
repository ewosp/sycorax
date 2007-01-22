using System;
using System.Collections.Generic;
using System.Text;

namespace Sycorax {
    /// <summary>
    /// MessageEventArgs
    /// </summary>
    public class TimestampMessageEventArgs : EventArgs {
        public TimestampMessageEventArgs (DateTime time, string message) {
            this.message = message;
            this.time = time;
        }

        private DateTime time;

        private string message;

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message {
            get { return message; }
        }

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <value>The time.</value>
        public DateTime Time {
            get { return time; }
        }

        public override string ToString () {
            return String.Format("[{0}] {1}", DateTime.Now.ToLongTimeString(), message);
        }
    }
}
