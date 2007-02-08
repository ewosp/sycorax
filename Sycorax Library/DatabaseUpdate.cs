/*
 * (c) Sébastien Santoro aka Dereckson, 2006, tous droits réservés
 *
 * Date: 13/07/2006
 * Time: 20:09
 *
 */

using System;
using ID3COM;
using System.IO;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace Sycorax {
    /// <summary>
    /// Mise à jour de la base de données
    /// </summary>
    public class DatabaseUpdate : IDisposable {
        public DatabaseUpdate (string connectionString) {
            //TODO : initalisation connexion MySQL à partir des préférences
            conn = new MySqlConnection(connectionString);
            conn.Open();
        }

        MySqlConnection conn;

        /// <summary>
        /// Parses the file properties.
        /// </summary>
        /// <param name="file">The file to analyze.</param>
        /// <returns>Instance of TuneToIndex containing file properties</returns>
        public TuneToIndex ParseProperties (string fileName) {
            TuneToIndex tune = new TuneToIndex();
            tune.Path = Path.GetFullPath(fileName);

            //This is temporary code for Megalo, a sample of logic to parse filename
            //In release version, of course, we should read global and local preferences to know how to read filename

            string nom = Path.GetFileNameWithoutExtension(fileName);
            //Recherche de ()
            Match match = Regex.Match(fileName, @"\(.*\)");
            if (match.Success) {
                //On prend l'intérieur des () en commentaire
                tune.Comment = match.Value.Substring(1, match.Length - 2);
                //Et on l'efface du nom
                nom = nom.Replace(match.Value, "").Trim();
            }
            //Le nom contient-il un tiret ?
            if (nom.Contains("-")) {
                string[] morceau = nom.Split(new char[1] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (morceau.Length == 1) {
                    //Euh le tiret n'était précédé ou suivi de rien :/
                    tune.Title = morceau[0];
                } else {
                    //Et voici le cas normal :)
                    tune.By = morceau[0];
                    tune.Title = morceau[1];
                }
            } else {
                //Titre
                tune.Title = nom;
            }

            return tune;
        }

        /// <summary>
        /// Revérifie les propriétés d'un fichier (tags idv3, durée) qui vient d'être modifié
        /// </summary>
        /// <param name="file">fichier modifié</param>
        public void RecheckProperties (string file) {
            //ID3ComTagClass Tag = id3lib.GetTag(file);
            //Log(String.Format("File updated: {0}", file));
        }

        /// <summary>
        /// Ajoute un nouveau fichier
        /// </summary>
        /// <param name="file">fichier à ajouter</param>
        public void AddFile (string file) {
            //We ignore directories
            if (Directory.Exists(file)) return;

            TuneToIndex tune = ParseProperties(file);
            string sql; MySqlCommand cmd;
            int TuneID;

            //1 - add tune if needed and get its ID
            if (IsTuneExists(tune)) {
                TuneID = GetTuneID(tune);
            } else {
                sql = String.Format(
                    @"INSERT INTO Tunes (tune_by, tune_title, tune_comment)
                      VALUES ('{0}', '{1}', '{2}')",
                    Utilities.SqlEscape(tune.By),
                    Utilities.SqlEscape(tune.Title),
                    Utilities.SqlEscape(tune.Comment)
                 );
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                TuneID = LastInsertID();
                
                SqlLog(sql);
                Log(String.Format("Tune added: {0} [#{1}]", tune, TuneID));
            }

            //2 - add file
            sql = String.Format(
                "INSERT INTO Files (file_path, tune_id) VALUES ('{0}', '{1}')",
                Utilities.SqlEscape(tune.Path),
                TuneID
            );

            cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            SqlLog(sql);
            Log(String.Format("File added: {0}", file));
        }

        /// <summary>
        /// Determines whether the specified tune exists.
        /// </summary>
        /// <param name="tune">The tune.</param>
        /// <returns>
        /// 	<c>true</c> if [is tune exists] [the specified tune]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsTuneExists (TuneToIndex tune) {
            string sql = String.Format(
                @"SELECT count(*) FROM Tunes
                  WHERE tune_by = '{0}' AND tune_title = '{1}' AND tune_comment = '{2}'",
                Utilities.SqlEscape(tune.By),
                Utilities.SqlEscape(tune.Title),
                Utilities.SqlEscape(tune.Comment)
            );
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            int count = reader.GetInt32(0);
            reader.Close();
            cmd.Dispose();
            SqlLog(sql);
            //Si count(*) = 1, nous renvoyons true, sinon false
            return (count > 0);
        }

        /// <summary>
        /// Gets the tune ID.
        /// </summary>
        /// <param name="tune">The tune to find.</param>
        /// <returns>The tune ID</returns>
        private int GetTuneID (TuneToIndex tune) {
            string sql = String.Format(
                @"SELECT tune_id FROM Tunes
                  WHERE tune_by = '{0}' AND tune_title = '{1}' AND tune_comment = '{2}'",
                Utilities.SqlEscape(tune.By),
                Utilities.SqlEscape(tune.Title),
                Utilities.SqlEscape(tune.Comment)
            );
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            int id = reader.GetInt32(0);
            reader.Close();
            cmd.Dispose();
            SqlLog(sql);
            return id;
        }

        /// <summary>
        /// Supprime un fichier
        /// </summary>
        /// <param name="file">fichier à supprimer</param>
        /// <param name="deleteTuneIfOrphan">supprime également le morceau si celui-ci est orphelin</param>
        public void DelFile (string file, bool deleteTuneIfOrphan) {
            int TuneID = -1;
            string sql; MySqlCommand cmd;

            string filepath = Utilities.SqlEscape(Path.GetFullPath(file));

            if (deleteTuneIfOrphan) {
                //Before delete the record, we've to get TuneID
                sql = String.Format(
                    "SELECT tune_id FROM Files WHERE file_path = '{0}'",
                    filepath
                );
                cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    TuneID = reader.GetInt32(0);
                } else {
                    //Tune has already been deleted
                    deleteTuneIfOrphan = false;
                }
                reader.Close();
                cmd.Dispose();
                SqlLog(sql);
            }

            //Delete file
            sql = String.Format(
                "DELETE FROM Files WHERE file_path = '{0}'",
                filepath
            );
            cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            SqlLog(sql);
            Log(String.Format("File deleted: {0}", file));

            if (deleteTuneIfOrphan) {
                //Is this tune really orphan ?
                sql = "SELECT count(*) FROM Files WHERE tune_id = " + TuneID;
                cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                int howMany = reader.GetInt32(0);
                reader.Close();
                cmd.Dispose();
                SqlLog(sql);

                if (howMany == 0) {
                    //Yes, so we can really delete it :
                    sql = "DELETE FROM Tunes WHERE tune_id = " + TuneID;
                    cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    SqlLog(sql);
                    Log(String.Format("Orphan tune #{0} has been deleted", TuneID));
                }
            }
        }

        /// <summary>
        /// Met à jour le path d'un fichier déplacé ou renommé
        /// </summary>
        /// <param name="oldPath">ancien path</param>
        /// <param name="newPath">nouveau path</param>
        public void MoveFile (string oldPath, string newPath) {
            string logEntry, sql;

            if (!Directory.Exists(newPath)) {
                //A file ? Easy, just one row to update :)
                sql = String.Format(
                    "UPDATE Files SET file_path = '{0}' WHERE file_path = '{1}'",
                    Utilities.SqlEscape(Path.GetFullPath(newPath)),
                    Utilities.SqlEscape(Path.GetFullPath(oldPath))
                );
                logEntry = String.Format("File moved: {0} -> {1}", oldPath, newPath);
            } else {
                /*
                 * Arg ... we've a massive update to do :p 
                 * We've to rename d:\oldpath\tune.mp3 into d:\newpath\tune.mp3
                 * - WHERE file_path = '{1}%' selects all files beginning with {1} ie newPath
                 * - new file_path begins with newPath and ends with the filename
                 *   = CONCAT(newPath, filename)
                 *   = CONCAT('{0}\\\\', filename)
                 *   = CONCAT('{0}\\\\', end of '{1}')
                 *     the end of '{1}' is the substring from the character following oldPath
                 *   = CONCAT('{0}\\\\', SUBSTRING(file_path FROM the character following oldPath)
                 *   = CONCAT('{0}\\\\', SUBSTRING(file_path FROM LENGTH('{0}') + 1)
                 *
                 *     Oh, why {0}\\\\ ? To add a trailing \ at our newPath.
                 *     Here we've 4 \, one escaped for MySQL, one for C#
                 *     Yes, if we've to declare a regexp expression in a sql statement without SqlEscape() call, we'll need \\\\\\\\ :^p
                 */
                sql = String.Format(
                    "UPDATE Files SET file_path = CONCAT('{0}\\\\', SUBSTRING(file_path FROM LENGTH('{0}') + 1)) WHERE file_path LIKE '{1}%'",
                    Utilities.SqlEscape(Path.GetFullPath(newPath)),
                    //LIKE ... is a regexp, so we need to double escape :
                    //TODO : replace oneUtilities.SqlEscape by a escape regex compliant
                    Utilities.SqlEscape(Utilities.SqlEscape(Path.GetFullPath(oldPath)))
                );
                logEntry = String.Format("Directory updated: {0} -> {1}", oldPath, newPath);
            }

            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            SqlLog(sql);
            Log(logEntry);

        }

        #region MySQL functions
        /// <summary>
        /// Get the last insert ID.
        /// </summary>
        /// <returns>last insert ID</returns>
        private int LastInsertID () {
            string sql = "SELECT LAST_INSERT_ID()";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            int returnValue = reader.GetInt32(0);
            reader.Close();
            cmd.Dispose();
            return returnValue;
        }
        #endregion

        #region IDisposable Members

        public void Dispose () {
            conn.Close();
            conn.Dispose();
        }

        #endregion

        /// <summary>
        /// Truncates the index tables (Files and Tunes).
        /// </summary>
        public void TruncateIndex () {
            //Truntace files table            
            string sql = "TRUNCATE Files";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            SqlLog(sql);
            Log("Tables Files has been truncated.");

            //Truntace tunes table
            sql = "TRUNCATE Tunes";
            cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            SqlLog(sql);
            Log("Tables Tunes has been truncated.");
        }

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
    }
}