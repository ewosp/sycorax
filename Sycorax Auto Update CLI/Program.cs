/*
 * This command line application is a substitution to AutoUpdate service.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;

namespace Sycorax.AutoUpdateCLI {
    /// <summary>
    /// Main class
    /// </summary>
    class Program {
        
        /// <summary>
        /// Thread principal
        /// </summary>
        static public void Do () {
            try {
                //Lecture des préférences
                Console.WriteLine("[*] Loading options ...");
                options = MainOptions.Load();

                //Initialisation de la base de données et de sa classe de mise à jour
                Console.WriteLine("[*] Initializing database connection and update facilities ...");
                databaseUpdate = new DatabaseUpdate(options.ConnectionString);
                databaseUpdate.PrintConsoleOutput = true;

                //Lancement d'une surveillance pour chaque dossier repris dans l'option FoldersToWatch
                Console.WriteLine("[*] Initializing folders watch ...");

                //Let's check if we've folders to watch
                //  CLI is very useful to test if DatabaseUpdate initialization is OK,
                //  it's why we don't test options.FoldersToWatch.Length before !
                if (options.FoldersToWatch.Length == 0) {
                    //Nothing to do
                    Console.WriteLine("    ( no folder to watch, exiting )");
                    databaseUpdate.Dispose();
                    return;
                }
                watchers = new SurveillanceDossiers[options.FoldersToWatch.Length];
                for (int i = 0 ; i < options.FoldersToWatch.Length ; i++) {
                    watchers[i] = new SurveillanceDossiers(options.FoldersToWatch[i], databaseUpdate);
                    watchers[i].Error += new EventHandler<ExceptionEventArgs>(Program_Error);
                    Console.WriteLine("    ° " + options.FoldersToWatch[i]);
                }

                Console.WriteLine("______________________________________________________________________");
                Console.WriteLine();

                //Oki, manage events


            } catch (Exception e) {
                //Options XML deserialize InnerException
                if (e.InnerException != null) {
                    e = e.InnerException;
                }
                Console.WriteLine(e.Message);
                if (options != null && options.DebugMode) {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        static void Main (string[] args) {
            PrintCredits();
            
            //Start a new thread
            Thread thread = new Thread(new ThreadStart(Do));
            thread.Start();

            //Let's wait the thread is alive
            while (!thread.IsAlive);

            while (true);
        }

        static void Program_Error (object sender, ExceptionEventArgs e) {
            Console.WriteLine(e.RaisedException.Message);
        }

        private static void PrintCredits () {
            Console.WriteLine("Sycorax Auto Update {0} -- Command Line Interface", Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("(c) 2006, Espace Win Open Source Project");
            Console.WriteLine("Project leader : Sébastien Santoro aka Dereckson [DcK]");
            Console.WriteLine(".:: Radio #Win Technical Preview ::.");
            Console.WriteLine("______________________________________________________________________");
            Console.WriteLine();
        }

        /// <summary>
        /// Surveillances
        /// </summary>
        private static SurveillanceDossiers[] watchers;

        /// <summary>
        /// Composant de mise à jour de la base de données
        /// </summary>
        private static DatabaseUpdate databaseUpdate;

        /// <summary>
        /// Options du service
        /// </summary>
        private static MainOptions options;
    }
}
