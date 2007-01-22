using System;
using System.Collections.Generic;
using System.Text;

namespace Sycorax {
    /// <summary>
    /// MessageEventArgs
    /// </summary>
    public class TimestampMessageEventArgs : MessageEventArgs {
        public TimestampMessageEventArgs (DateTime time, string message) : base(message) {
            this.time = time;
        }

        private DateTime time;

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <value>The time.</value>
        public DateTime Time {
            get { return time; }
        }

        public override string ToString () {
            return String.Format("[{0}] {1}", DateTime.Now.ToLongTimeString(), Message);
        }
    }
}
