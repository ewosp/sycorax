/*
 * (c) Sébastien Santoro aka Dereckson, 2006, tous droits réservés
 *
 * Date: 13/07/2006
 * Time: 19:58
 *
 */

using System;
using System.IO;


namespace Sycorax {

    /// <summary>
    /// Description of Surveillance.
    /// </summary>
    public class SurveillanceDossiers : IDisposable {
        /// <summary>
        /// Initialise une nouvelle surveillance de dossiers
        /// </summary>
        /// <param name="path">dossier à surveiller</param>
        /// <param name="databaseUpdate">Database update component.</param>
        public SurveillanceDossiers (string path, DatabaseUpdate databaseUpdate) {
            watcher = new FileSystemWatcher(path);
            watcher.EnableRaisingEvents = true;
            //watcher.Filter
            watcher.Changed += delegate(object sender, FileSystemEventArgs e) { databaseUpdate.RecheckProperties(e.FullPath); };
            watcher.Created += delegate(object sender, FileSystemEventArgs e) { databaseUpdate.AddFile(e.FullPath); };
            watcher.Deleted += delegate(object sender, FileSystemEventArgs e) { databaseUpdate.DelFile(e.FullPath, false); };
            watcher.Error += delegate(object sender, ErrorEventArgs e) {Error(this, new ExceptionEventArgs(e.GetException())); };
            watcher.Renamed += delegate(object sender, RenamedEventArgs e) { databaseUpdate.MoveFile(e.OldFullPath, e.FullPath); };
        }

        private FileSystemWatcher watcher;

        /// <summary>
        /// Occurs when the internal buffer overflows.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> Error;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose () {
            watcher.Dispose();
        }
    }
}