using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Sycorax {
    /// <summary>
    /// Probe MySQL connection
    /// </summary>
    public class ProbeMySqlConnection : IDisposable {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ProbeMySqlConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ProbeMySqlConnection (string connectionString) {
            conn = new MySqlConnection();
            conn.ConnectionString = connectionString;
            conn.InfoMessage += new MySqlInfoMessageEventHandler(conn_InfoMessage);
            conn.StateChange += new System.Data.StateChangeEventHandler(conn_StateChange);
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public void Open () {
            try {
                conn.Open();
            } catch (Exception e) {
                Error(this, new ExceptionEventArgs(e));
            }
        }

        /// <summary>
        /// Gets the connection status.
        /// </summary>
        /// <value>The connection status.</value>
        public string Status {
            get {
                return conn.State.ToString();
            }
        }

        #region Events
        /// <summary>
        /// Handles the StateChange event of the conn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Data.StateChangeEventArgs"/> instance containing the event data.</param>
        private void conn_StateChange (object sender, StateChangeEventArgs e) {
            //TODO : check the string representation of CurrentState enum
            StatusChange(this, new MessageEventArgs(e.CurrentState.ToString()));
        }

        /// <summary>
        /// Handles the InfoMessage event of the conn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="T:MySql.Data.MySqlClient.MySqlInfoMessageEventArgs"/> instance containing the event data.</param>
        private void conn_InfoMessage (object sender, MySqlInfoMessageEventArgs args) {
            //Gets last MySQL error and format it
            MySqlError lastError = args.errors[args.errors.Length - 1];
            string error = String.Format(
                "Error #{0} - {2} [{1}]",
                lastError.Code,
                lastError.Level,
                lastError.Message
            );
            
            //Throws an exception to our Error event
            Error(this, new ExceptionEventArgs(new Exception(error)));
            
        }

        /// <summary>
        /// Occurs when an exception is thrown or MySQL server complains about errors
        /// </summary>
        public event EventHandler<ExceptionEventArgs> Error;

        /// <summary>
        /// Occurs when status connection changes
        /// </summary>
        public event EventHandler<MessageEventArgs> StatusChange;
        #endregion

        /// <summary>
        /// MySQL connection to probe
        /// </summary>
        private MySqlConnection conn;

        #region IDisposable Members

        public void Dispose () {
            conn.Close();
            conn.Dispose();
        }

        #endregion
    }
}
