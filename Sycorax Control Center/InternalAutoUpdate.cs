using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace Sycorax.ControlCenter {
	public class InternalAutoUpdate {
		public InternalAutoUpdate () {
			if (Program.databaseUpdate == null) Program.databaseUpdate = new DatabaseUpdate(Program.options.ConnectionString);
		}

		/// <summary>
		/// Démarrage de la surveillance
		/// </summary>
		private void StartSurveillance () {
			//Lancement d'une surveillance pour chaque dossier repris dans l'option FoldersToWatch
			watchers = new SurveillanceDossiers[Program.options.FoldersToWatch.Length];
			for (int i = 0 ; i < Program.options.FoldersToWatch.Length ; i++) {
				watchers[i] = new SurveillanceDossiers(Program.options.FoldersToWatch[i], Program.databaseUpdate, Program.options.DeleteTunesIfOrphan);
				watchers[i].Error += new EventHandler<ExceptionEventArgs>(surveillanceDossiers_Error);
			}
            enabled = true;
		}

		/// <summary>
		/// Handles the Error event of the surveillanceDossiers control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:Sycorax.AutoUpdate.ExceptionEventArgs"/> instance containing the event data.</param>
		void surveillanceDossiers_Error (object sender, ExceptionEventArgs e) {
			//Log de l'erreur dans l'event viewer
			EventLog.WriteEntry(e.RaisedException.Source, e.RaisedException.Message, EventLogEntryType.Error);
			//Si le mode debug est actif, on affiche également le stacktrace dans une boîte de dialogue
			if (Program.options.DebugMode) {
				MessageBox.Show(e.RaisedException.StackTrace, e.RaisedException.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// Stops the surveillance.
		/// </summary>
		private void StopSurveillance () {
            if (watchers == null || watchers.Length == 0) return;
			foreach (SurveillanceDossiers watcher in watchers) {
				watcher.Dispose();
			}
            enabled = false;
		}

		private bool enabled;
		/// <summary>
		/// Gets or sets if internal auto update is enabled.
		/// </summary>
		public bool Enabled {
			get {
				return enabled;
			}
			set {
				if (enabled == value) {
					//Nothing to do
				} else if (value == true) {
					//Ok, let's enable
					StartSurveillance();
				} else {
					//Ok, let's disable
					StopSurveillance();
				}
			}
		}

		/// <summary>
		/// Surveillances
		/// </summary>
		private SurveillanceDossiers[] watchers;
	}
}
