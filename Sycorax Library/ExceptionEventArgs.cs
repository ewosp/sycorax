using System;
using System.Collections.Generic;
using System.Text;

namespace Sycorax {
    /// <summary>
    /// ExceptionEventArgs
    /// </summary>
    public class ExceptionEventArgs : EventArgs {
        public ExceptionEventArgs (Exception ex) {
            this.ex = ex;
        }

        private Exception ex;

        /// <summary>
        /// Gets the raised exception.
        /// </summary>
        /// <value>The raised exception.</value>
        public Exception RaisedException {
            get { return ex; }
        }
    }
}
