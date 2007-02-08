/*
 * (c) Sébastien Santoro aka Dereckson, 2006, tous droits réservés
 *
 * Date: 13/07/2006
 * Time: 19:47
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using Sycorax;

namespace Sycorax.AutoUpdate {

    /// <summary>
    /// Service de mise à jour automatique
    /// </summary>
    public class AutoUpdateService : ServiceBase {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AutoUpdateService"/> class.
        /// </summary>
        public AutoUpdateService () {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent () {
            this.ServiceName = "SycoraxAutoUpdate";
            this.AutoLog = true;
            this.CanPauseAndContinue = true;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose (bool disposing) {
            // TODO: Add cleanup code here (if required)
            base.Dispose(disposing);
        }

        /// <summary>
        /// Start this service.
        /// </summary>
        protected override void OnStart (string[] args) {           
            //Lecture des préférences
            options = MainOptions.Load();
            //Initialisation de la base de données et de sa classe de mise à jour
            databaseUpdate = new DatabaseUpdate(options.ConnectionString);
            //Lancement de la surveillance des dossiers
            StartSurveillance();
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnCustomCommand(System.Int32)"></see> executes when the Service Control Manager (SCM) passes a custom command to the service. Specifies actions to take when a command with the specified parameter value occurs.
        /// </summary>
        /// <param name="command">
        ///     The command message sent to the service :
        ///         1) Rehash des préférences et redémarrage de la surveillance.
        ///         2) Réinitialisation de la connexion databaseUpdate
        ///         3) 1 + 2
        /// </param>
        protected override void OnCustomCommand (int command) {
            switch (command) {
                case 3:
                    //1 + 2
                    options = MainOptions.Load();
                    goto case 2;
                    //Tiens, ça faisait bien une bonne dizaine
                    //d'années que je n'avais plus goto :p ^^

                case 2:
                    //Réinitialisation de la connexion databaseUpdate
                    databaseUpdate.Dispose();
                    databaseUpdate = new DatabaseUpdate(options.ConnectionString);
                    StopSurveillance();
                    StartSurveillance();
                    break;

                case 1:
                    //Rehash des préférences et redémarrage de la surveillance.
                    options = MainOptions.Load();
                    StopSurveillance();
                    StartSurveillance();
                    break;

                default:
                    throw new ArgumentException("Commande inconnue : " + command.ToString());
            }
        }

        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop () {
            options.Save();
            StopSurveillance();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Pause command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service pauses.
        /// </summary>
        protected override void OnPause () {
            StopSurveillance();
        }

        /// <summary>
        /// When implemented in a derived class, <see cref="M:System.ServiceProcess.ServiceBase.OnContinue"></see> runs when a Continue command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service resumes normal functioning after being paused.
        /// </summary>
        protected override void OnContinue () {
            StartSurveillance();
        }

        /// <summary>
        /// Démarrage de la surveillance
        /// </summary>
        private void StartSurveillance () {
            //Lancement d'une surveillance pour chaque dossier repris dans l'option FoldersToWatch
            watchers = new SurveillanceDossiers[options.FoldersToWatch.Length];
            for (int i = 0 ; i < options.FoldersToWatch.Length ; i++) {
                watchers[i] = new SurveillanceDossiers(options.FoldersToWatch[i], databaseUpdate, options.DeleteTunesIfOrphan);
                watchers[i].Error += new EventHandler<ExceptionEventArgs>(surveillanceDossiers_Error);
            }
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
            if (options.DebugMode) {
                MessageBox.Show(e.RaisedException.StackTrace, e.RaisedException.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Stops the surveillance.
        /// </summary>
        private void StopSurveillance () {
            foreach (SurveillanceDossiers watcher in watchers) {
                watcher.Dispose();
            }
        }

        /// <summary>
        /// Surveillances
        /// </summary>
        private SurveillanceDossiers[] watchers;

        /// <summary>
        /// Composant de mise à jour de la base de données
        /// </summary>
        private DatabaseUpdate databaseUpdate;

        /// <summary>
        /// Options du service
        /// </summary>
        private MainOptions options;

    }
}