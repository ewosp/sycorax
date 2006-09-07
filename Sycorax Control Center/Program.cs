/*
 * (c) Sébastien Santoro aka Dereckson, 2006, tous droits réservés
 *
 * Date: 14/07/2006
 * Time: 16:53
 *
 */

using System;
using System.Windows.Forms;

namespace Sycorax.IndexBuilder {

    static internal class Program {
        /// <summary>
        /// Point d'entrée principal de l'application
        /// </summary>
        /// <param name="args">The args.</param>
        [STAThread]
        public static void Main (string[] args) {
            if (Start()) {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try {
                    Application.Run(new MainForm());
                    Stop();
                } catch (Exception e) {
                    //TODO : bug report
                    MessageBox.Show(
                        String.Format(
                            "Exception : {0}\n\n{1}\n\nNous vous invitons à signaler ce bug sur http://bugzilla.espace-win.org/",
                            e.Message, e.StackTrace
                        ),
                        "Erreur fatale", MessageBoxButtons.OK, MessageBoxIcon.Stop
                    );
                }
            }
        }

        /// <summary>
        /// Initializes the components needed by the application.
        /// </summary>
        /// <returns>if true, we can launch application</returns>
        private static bool Start () {
            //Lecture des préférences
            try {
                options = MainOptions.Load();
            } catch (Exception e) {
                //90% of chance to get a InvalidOperationException containing the XML related exception in InnerException property
                if (e.InnerException != null) {
                    e = e.InnerException;
                }
                //
                DialogResult result = MessageBox.Show(
                    String.Format(
                        "Fichier de préférence : {0}\n\nErreur : {1}\n\nSi vous venez de modifier manuellement votre fichier, cliquez sur NON, repérez votre erreur et corrigez-la avant de relancer Sycorax.\n\nSouhaitez-vous EFFACER ce fichier et le REMPLACER PAR UNE VERSION PAR DEFAUT ?\n\nSi vous cliquez sur OUI, l'ancien fichier de préférence sera détruit, le nouveau fichier de préférences contiendra les options par défaut.\nAttention, cette opération est irréversible et toutes vos préférences (par exemple, la liste de vos dossiers) seront perdues.\n\nSi vous cliquez sur NON, il vous sera alors loisible d'éditer manuellement le fichier de préférence pour corriger l'erreur. Notre support technique se fera un plaisir de vous y aider, le cas échéant.\n\nSi vous n'arrivez pas à prendre une décision, nous vous recommandons de cliquer sur NON et de déplacer votre fichier sur le bureau avant de relancer Sycorax.\nVous pourrez ainsi par la suite effectuer toutes les modifications nécessaires, par exemple recopier votre liste de dossiers depuis l'ancien fichier vers le nouveau.",
                        MainOptions.DefaultOptionsFile, e.Message
                    ),
                    "[IMPORTANT] Votre fichier de préférence est corrompu.",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2
                );
                if (result == DialogResult.Yes) {
                    options = MainOptions.CreateNewOptionsFile(MainOptions.DefaultOptionsFile);
                } else {
                    return false;
                }
            }

            //Kludge
            //options.ConnectionString = "Server=AAmiens-154-1-79-121.w86-208.abo.wanadoo.fr;Database=RadioWin;Uid=Sycorax;Pwd=;";

            return true;
        }

        /// <summary>
        /// Disposes the components used by the application.
        /// </summary>
        private static void Stop () {
            options.Save();
        }

        /// <summary>
        /// Main options
        /// </summary>
        public static MainOptions options;
    }
}