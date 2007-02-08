using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Sycorax {
    /// <summary>
    /// Query the database
    /// </summary>
    public class DatabaseQuery : IDisposable {

        #region Constructor, Dispose(), properties
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseQuery"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public DatabaseQuery (string connectionString) {
            conn = new MySqlConnection(connectionString);
            conn.Open();
        }

        /// <summary>
        /// MySQL connection
        /// </summary>
        private MySqlConnection conn;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose () {
            conn.Close();
            conn.Dispose();
        }
        #endregion

        #region Logging
        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="message">message to log</param>
        public void Log (string message) {
            if (LogEntry != null) {
                LogEntry(this, new TimestampMessageEventArgs(DateTime.Now, message));
            }
        }

        public void SqlLog (string query) {
            if (SqlQuery != null) {
                SqlQuery(this, new TimestampMessageEventArgs(DateTime.Now, query));
            }
        }

        public event EventHandler<TimestampMessageEventArgs> LogEntry;
        public event EventHandler<TimestampMessageEventArgs> SqlQuery;
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the tune info.
        /// </summary>
        /// <param name="path">The path to the tune.</param>
        /// <returns>Tune info</returns>
        /// <exception cref="System.Exception">throw if path is not found in database</exception>
        public TuneToIndex GetTuneInfo (string path) {
            bool notFoundException = true;
            string sql = String.Format(
                "SELECT t.tune_by, t.tune_title, t.tune_comment FROM tunes t, files f WHERE t.tune_id = f.file_id AND f.file_path '{0}'",
                Utilities.SqlEscape(path)
            );
            TuneToIndex tune = new TuneToIndex();
            tune.Path = path;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) {
                notFoundException = false;
                tune.By = reader.GetString(0);
                tune.Title = reader.GetString(1);
                tune.Comment = reader.GetString(2);
            }
            reader.Close();
            cmd.Dispose();
            if (notFoundException) throw new Exception("Not found in database");
            return tune;
        }
        #endregion

    }
}
