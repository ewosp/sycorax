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
        /// Initializes a new instance of the <see cref="SurveillanceDossiers"/> class.
        /// </summary>
        /// <param name="path">The path to the folder to watch.</param>
        /// <param name="databaseUpdate">The database update component.</param>
        public SurveillanceDossiers (string path, DatabaseUpdate databaseUpdate)
            : this(path, databaseUpdate, false) {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveillanceDossiers"/> class.
        /// </summary>
        /// <param name="path">The path to the folder to watch.</param>
        /// <param name="databaseUpdate">The database update component.</param>
        /// <param name="deleteTuneIfOrphan">if set to <c>true</c>, deletes tune if it's orphan.</param>
        public SurveillanceDossiers (string path, DatabaseUpdate databaseUpdate, bool deleteTuneIfOrphan)
            : this(path, databaseUpdate, deleteTuneIfOrphan, true) {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurveillanceDossiers"/> class.
        /// </summary>
        /// <param name="path">The path to the folder to watch.</param>
        /// <param name="databaseUpdate">The database update component.</param>
        /// <param name="deleteTuneIfOrphan">if set to <c>true</c>, deletes tune if it's orphan.</param>
        /// <param name="indexSubdirectories">if set to <c>true</c> indexes also subdirectories</param>
        public SurveillanceDossiers (string path, DatabaseUpdate databaseUpdate, bool deleteTuneIfOrphan, bool indexSubdirectories) {
            watcher = new FileSystemWatcher(path);
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = indexSubdirectories;
            //watcher.Filter
            watcher.Changed += delegate(object sender, FileSystemEventArgs e) { databaseUpdate.RecheckProperties(e.FullPath); };
            watcher.Created += delegate(object sender, FileSystemEventArgs e) { databaseUpdate.AddFile(e.FullPath); };
            watcher.Deleted += delegate(object sender, FileSystemEventArgs e) { databaseUpdate.DelFile(e.FullPath, deleteTuneIfOrphan); };
            watcher.Error += delegate(object sender, ErrorEventArgs e) {
                if (Error != null) Error(this, new ExceptionEventArgs(e.GetException()));
            };
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