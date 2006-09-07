using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Text;
using System.IO;

namespace Sycorax.ControlCenter {
	/// <summary>
	/// Application Main form 
	/// </summary>
	public partial class MainForm {
		public MainForm () {
			InitializeComponent();

			//Autoupdate :: we need to initialize our ServiceController
			sc = new ServiceController("SycoraxAutoUpdate");

			//Config tab :: initializes the controls, reading options
			RefreshOptionsArea();
		}

		/// <summary>
		/// SycoraxAutoUpdate Service Controller
		/// </summary>
		private ServiceController sc;

		/// <summary>
		/// Indique si les options ont �t� modifi�es
		/// </summary>
		private bool optionsHasBeenModified;

		/// <summary>
		/// Indique si les options ont �t� modifi�es
		/// </summary>
		public bool OptionsHasBeenModified {
			get {
				return optionsHasBeenModified;
			}
			set {
				//Si les options ont �t� modifi�es, alors le bouton enregistrer est activ�.
				buttonEnregistrerOptions.Enabled = optionsHasBeenModified = value;
			}
		}

		/// <summary>
		/// Handles the Enter event of the tabPageAutoUpdate control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void tabPageAutoUpdate_Enter (object sender, EventArgs e) {
			//Mise � jour de la zone du haut, c�d du contr�leur de service
			ProbeService();
		}

		/// <summary>
		/// V�rifie l'�tat du service
		/// </summary>
		private void ProbeService () {
			try {
				switch (sc.Status) {
					//Affichage de l'�tat dans labelStatutAutoUpdateServiceResult
					//Les tags que nous d�finissons correspondent aux op�rations
					//attendues pour ces liens.
					case ServiceControllerStatus.ContinuePending:
						labelStatutAutoUpdateServiceResult.Text = "Reprise en cours";
						linkLabelServicePauseContinue.Tag = "Pause";
						linkLabelServicePlayStop.Tag = "Stop";
						break;
					case ServiceControllerStatus.Paused:
						labelStatutAutoUpdateServiceResult.Text = "Pause";
						linkLabelServicePauseContinue.Tag = "Continue";
						linkLabelServicePlayStop.Tag = "Stop";
						break;
					case ServiceControllerStatus.PausePending:
						labelStatutAutoUpdateServiceResult.Text = "Pause en cours";
						linkLabelServicePauseContinue.Tag = "Continue";
						linkLabelServicePlayStop.Tag = "Stop";
						break;
					case ServiceControllerStatus.Running:
						labelStatutAutoUpdateServiceResult.Text = "Actif";
						linkLabelServicePauseContinue.Tag = "Pause";
						linkLabelServicePlayStop.Tag = "Stop";
						break;
					case ServiceControllerStatus.StartPending:
						labelStatutAutoUpdateServiceResult.Text = "Lancement en cours";
						linkLabelServicePauseContinue.Tag = "Pause";
						linkLabelServicePlayStop.Tag = "Stop";
						break;
					case ServiceControllerStatus.Stopped:
						labelStatutAutoUpdateServiceResult.Text = "Arr�t�";
						linkLabelServicePauseContinue.Tag = "";
						linkLabelServicePlayStop.Tag = "Start";
						break;
					case ServiceControllerStatus.StopPending:
						labelStatutAutoUpdateServiceResult.Text = "Arr�t en cours";
						linkLabelServicePauseContinue.Tag = "";
						linkLabelServicePlayStop.Tag = "Start";
						break;
				}
				labelStatutAutoUpdateServiceResult.ForeColor = (sc.Status == ServiceControllerStatus.Running) ? Color.DarkGreen : SystemColors.ControlText;
				//Mise � jour du texte des liens
				UpdateServiceLinks();
			} catch (Exception e) {
				if (Program.options.DebugMode) {
					PrintException(e);
				}
				labelAutoUpdateException.Text = e.Message;

				//En cas d'erreur sur le service, les liens ne servent pas � grand chose
				linkLabelServicePlayStop.Hide();
				linkLabelServicePauseContinue.Hide();
			}
		}

		/// <summary>
		/// Updates the service links.
		/// </summary>
		private void UpdateServiceLinks () {
			//Lien Start/Stop
			//TODO : Lang
			//Mise � jour du nom du lien
			linkLabelServicePlayStop.Text = linkLabelServicePlayStop.Tag.ToString();

			//Lien Pause/Continue
			if (linkLabelServicePauseContinue.Tag.ToString() == "") {
				//Pas de tag (le service �tant � l'arr�t ou en cours d'arr�t), le lien dispara�t.
				linkLabelServicePauseContinue.Hide();
			} else {
				//On r�affiche �ventuellement le tag
				linkLabelServicePauseContinue.Show();
				//TODO : Lang
				//Mise � jour du nom du lien
				linkLabelServicePauseContinue.Text = linkLabelServicePauseContinue.Tag.ToString();
			}
		}

		/// <summary>
		/// Prints an exception.
		/// </summary>
		/// <param name="e">The exception.</param>
		private void PrintException (Exception e) {
			//Affichage de l'exception dans textBoxException
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(e.Message);
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine(e.StackTrace);
			textBoxException.Text = sb.ToString();
			//Si le tab n'�tait pas encore pr�sent, affichons-le
			if (!tabControlMain.Controls.Contains(tabPageDebug)) {
				//On ajoute l'onglet debug � droite
				this.tabControlMain.Controls.Add(tabPageDebug);
				//On zappe dessus
				//Attention : si l'on zappe syst�matiquement sur ce tab
				//d�s qu'une exception survient, il ne sera pas possible
				//de voir correctement les autres onglets si l'exception
				//se reproduit d�s qu'on entre dans un de ceux-ci comme
				//c'est par ex. le cas si le service n'est pas install�.
				tabControlMain.SelectedTab = tabPageDebug;
			}
		}

		/// <summary>
		/// Handles the LinkClicked event of the linkLabelServicePlayStop control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
		private void linkLabelServicePlayStop_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e) {
			try {
				LinkLabel linkLabel = (LinkLabel)sender;
				if (linkLabel.Tag.ToString() == "Play") {
					//D�marrage du service
					sc.Start();
				} else {
					//Arr�t du service
					sc.Stop();
				}
			} catch (Exception ex) {
				if (Program.options.DebugMode) {
					PrintException(ex);
				}
				labelAutoUpdateException.Text = ex.Message;
				return;
			}
			ProbeService();
		}

		/// <summary>
		/// Handles the LinkClicked event of the linkLabelServicePause control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
		private void linkLabelServicePause_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e) {
			LinkLabel linkLabel = (LinkLabel)sender;

			//Pas de tag ? il ne se passe rien (n'est pas sens� arriv� gr�ce � UpdateServiceLinks)
			if (linkLabel.Tag.ToString() == "") return;

			try {
				//Lisons le tag du lien pour d�terminer le comportement � adopter (pause ou continue)
				if (linkLabel.Tag.ToString() == "Pause") {
					//D�marrage du service
					sc.Pause();
				} else {
					//Arr�t du service
					sc.Continue();
				}
			} catch (Exception ex) {
				if (Program.options.DebugMode) {
					PrintException(ex);
				}
				labelAutoUpdateException.Text = ex.Message;
				return;
			}
			ProbeService();
		}

		/// <summary>
		/// Handles the CheckedChanged event of the checkBoxDeleteTunesIfOrphans control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void checkBoxDeleteTunesIfOrphans_CheckedChanged (object sender, EventArgs e) {
			CheckBox checkBox = (CheckBox)sender;
			Program.options.DeleteTunesIfOrphans = checkBox.Checked;
			OptionsHasBeenModified = true;
		}

		/// <summary>
		/// Handles the CheckedChanged event of the checkBoxDebugMode control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void checkBoxDebugMode_CheckedChanged (object sender, EventArgs e) {
			CheckBox checkBox = (CheckBox)sender;
			Program.options.DebugMode = checkBox.Checked;
			OptionsHasBeenModified = true;
		}

		/// <summary>
		/// Handles the Click event of the buttonEnregistrerOptions control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void buttonEnregistrerOptions_Click (object sender, EventArgs e) {
			SaveOptions();
		}

		/// <summary>
		/// Saves the options.
		/// </summary>
		private void SaveOptions () {
			try {
				Program.options.Save(MainOptions.DefaultOptionsFile);
				OptionsHasBeenModified = false;
			} catch (Exception ex) {
				if (Program.options.DebugMode) PrintException(ex);

				if (MessageBox.Show(ex.Message, "Impossible de sauvegader les options.", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Retry) {
					//Ok, let's retry
					SaveOptions();
				}
			}
		}

		private void checkedListBoxFolders_SelectedIndexChanged (object sender, EventArgs e) {
			CheckedListBox checkedListBox = (CheckedListBox)sender;
			//Activons le bouton suppression si au moins une case est s�lectionn�e
			buttonDeleteSelectedFolders.Enabled = (checkedListBox.CheckedItems.Count > 0);
		}

		private void textBoxFolderToAdd_TextChanged (object sender, EventArgs e) {
			TextBox textBox = (TextBox)sender;
			//Activons le bouton suppression si il y a du texte que c'est un dossier existant
			buttonAddFolder.Enabled = Directory.Exists(textBox.Text) || textBox.Text == "---";
		}

		private void buttonAddFolder_Click (object sender, EventArgs e) {
			if (textBoxFolderToAdd.Text == "---") {
				checkedListBoxFolders.Items.Clear();
				Program.options.FoldersToWatch = new string[] { };
				textBoxFolderToAdd.Clear();
			} else {
				AddFolder(textBoxFolderToAdd.Text);
			}
		}

		private void AddFolder (string path) {
			//Ajout dans la liste
			checkedListBoxFolders.Items.Add(path, CheckState.Unchecked);
			//On efface le texte de la textBox
			textBoxFolderToAdd.Clear();
			//On l'ajoute dans les options
			Program.options.AddFolder(path);
		}

		private void buttonBrowse_Click (object sender, EventArgs e) {
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "S�lectionnez le dossier � surveiller";
			if (Directory.Exists(textBoxFolderToAdd.Text)) {
				dialog.SelectedPath = textBoxFolderToAdd.Text;
			}
			dialog.ShowNewFolderButton = true;
			if (dialog.ShowDialog() == DialogResult.OK) {
				textBoxFolderToAdd.Text = dialog.SelectedPath;
			}
		}

		private void buttonDeleteSelectedFolders_Click (object sender, EventArgs e) {
			/*
			 * Instinctivement, le code suivant peut s'imposer :
			 * 
			 *   foreach (object o in checkedListBoxFolders.SelectedItems) {
			 *      DelFolder(o.ToString());
			 *   }
			 * 
			 * Autrement dit :
			 * 
			 *   foreach (object o in checkedListBoxFolders.SelectedItems) {
			 *      checkedListBoxFolders.Items.Remove(path);
			 *      //...
			 *   }
			 * 
			 * Le souci est que nous modifions la liste des items de checkedListBoxFolders
			 * au fur et � mesure que nous parcourons la boucle foreach, ce qui provoquera
			 * une exception InvalidOperationOperation tout � fait justifi�e.
			 * 
			 * La solution est assez simple, il suffit de copier les items s�lectionn�es
			 * dans un tableau temporaire et de parcourir non pas notre liste d'items s�-
			 * lectionn�s mais ce tableau o� nous venons de les copier.
			 */
			//Copions les items s�lectionn�s dans un tableau
			object[] itemsToDel = new object[checkedListBoxFolders.SelectedItems.Count];
			checkedListBoxFolders.SelectedItems.CopyTo(itemsToDel, 0);
			//Supprimons-les
			foreach (object o in itemsToDel) {
				DelFolder(o.ToString());
			}
		}


		/// <summary>
		/// Deletes the folder.
		/// </summary>
		/// <param name="path">The folder path.</param>
		private void DelFolder (string path) {
			//Ajout dans la liste
			checkedListBoxFolders.Items.Remove(path);
			//On le supprime dans les options
			Program.options.DelFolder(path);
		}


		/// <summary>
		/// Handles the Click event of the buttonRAZOptions control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void buttonRAZOptions_Click (object sender, EventArgs e) {
			if (MessageBox.Show(
				"�tes-vous s�r de vouloir restaurer les pr�f�rences par d�faut.\n\nSi vous r�pondez oui, votre liste de dossiers � surveiller sera perdue.",
				"Remise � z�ro des pr�f�rences",
				MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes
			) {
				//Ok, let's RAZ
				Program.options.SetDefaults();
				//Enregistrons directement
				Program.options.Save();
				//Hop on met � jour l'interface
				RefreshOptionsArea();
			}
		}

		#region Auto Update :: Manual auto update
		/// <summary>
		/// Handles the LinkClicked event of the linkInternalAutoUpdateUp control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.Windows.Forms.LinkLabelLinkClickedEventArgs"/> instance containing the event data.</param>
		private void linkInternalAutoUpdateUp_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e) {
			try {
				if (labelInternalAutoUpdateStatus.Text == "Off") {
					//Start our internal auto update
					
				} else {
					//Stop our interal auto update
				}
			} catch (Exception ex) {
				if (Program.options.DebugMode) {
					PrintException(ex);
				}
				return;
			}
			ProbeService();
		}

		#endregion

		#region Page Rebuild index

		/// <summary>
		/// Handles the Click event of the buttonStartRebuildIndex control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void buttonStartRebuildIndex_Click (object sender, EventArgs e) {
			//If we don't have any folder watched, alert the user he's going to destroy index
			if (Program.options.FoldersToWatch.Length == 0) {
				DialogResult result = MessageBox.Show(
					"Vous vous appr�tez � d�truire l'int�gralit� de votre index.\n\nVous n'avez d�fini aucun dossier en vue d'une r�indexation.\n\nConfirmez-vous cette ordre de totale destruction de l'index ?",
					"Confirmation avant auto-destruction",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2
				);
				if (result == DialogResult.No) {
					//No ? We cancel
					return;
				}
			}
			//Rebuild index
			try {
				RebuildIndex();
			} catch (Exception ex) {
				MessageBox.Show(
					String.Format(
						"L'index n'a pu �tre correctement reg�n�r�.\n\nException : {0}\n\n{1}Nous vous invitons � signaler ce bug sur http://bugzilla.espace-win.org/",
						ex.Message,
						Program.options.DebugMode ? ex.StackTrace + "\n\n" : ""
					),
					"Erreur durant l'indexation", MessageBoxButtons.OK, MessageBoxIcon.Stop
				);
			}
		}

		/// <summary>
		/// Rebuilds the index.
		/// </summary>
		private void RebuildIndex () {
			//Start
			buttonStartRebuildIndex.Enabled = false;
			progressBarRebuild.Visible = true;

			List<string> FilesToIndex = new List<string>();

			//Let's go
			foreach (string folder in Program.options.FoldersToWatch) {
				//get the audio file contained in each folder to watch
				FilesToIndex.AddRange(GetAudioFiles(folder, true));
			}

			//Now, we've to index all those file
			if (Program.databaseUpdate == null) Program.databaseUpdate = new DatabaseUpdate(Program.options.ConnectionString);
			Program.databaseUpdate.TruncateIndex();
			for (int i = 0 ; i < FilesToIndex.Count ; i++) {
				//Add file to index
				Program.databaseUpdate.AddFile(FilesToIndex[i]);
				//Update progress bar
				progressBarRebuild.Value = (i + 1) * 100 / FilesToIndex.Count;
			}
			//End
			buttonStartRebuildIndex.Enabled = true;
			progressBarRebuild.Visible = false;
		}

		/// <summary>
		/// Gets the audio files contained in specified folders.
		/// </summary>
		/// <param name="folder">The folder.</param>
		/// <param name="recursive">if set to <c>true</c> get also audio files in subfolders.</param>
		/// <returns>List of audio files</returns>
		private List<string> GetAudioFiles (string folder, bool recursive) {
			List<string> files = new List<string>();
			foreach (string file in Directory.GetFiles(folder)) {
				if (Program.options.isExtensionWatched(Path.GetExtension(file))) {
					files.Add(file);
				}
			}
			if (recursive) {
				foreach (string subfolder in Directory.GetDirectories(folder)) {
					files.AddRange(GetAudioFiles(subfolder, true));
				}
			}
			return files;
		}

		#endregion

		#region Page Config
		/// <summary>
		/// Handles the Enter event of the tabPageConfig control.
		/// Set available options page and fill controls from our options
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void tabPageConfig_Enter (object sender, EventArgs e) {
			listBoxOptions.Items.Clear();
			listBoxOptions.Items.AddRange(new object[] {
                GetPageOption(AvailablePageOption.General),
                GetPageOption(AvailablePageOption.Directories),
                GetPageOption(AvailablePageOption.Extensions),
                GetPageOption(AvailablePageOption.ID3),
                GetPageOption(AvailablePageOption.Database),
                GetPageOption(AvailablePageOption.Queries),
            });
		}

		/// <summary>
		/// Met � jour la zone des options du tab AutoUpdate
		/// </summary>
		private void RefreshOptionsArea () {
			//Page General
			checkBoxDebugMode.Checked = Program.options.DebugMode;
			checkBoxDeleteTunesIfOrphans.Checked = Program.options.DeleteTunesIfOrphans;

			//Page Database
			textBoxConnectionString.Text = Program.options.ConnectionString;

			//Page Directories
			checkedListBoxFolders.Items.AddRange(Program.options.FoldersToWatch);

			//Page Extensions

			//Page ID3

			//Page Queries
		}


		/// <summary>
		/// Handles the SelectedIndexChanged event of the listBoxOptions control.
		/// Show the correct control in right area
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void listBoxOptions_SelectedIndexChanged (object sender, EventArgs e) {
			ListBox listBox = (ListBox)sender;
			//Clear current option control
			splitContainer1.Panel2.Controls.Clear();
			//Set the new control
			PageOption pageOption = (PageOption)listBox.SelectedItem;
			splitContainer1.Panel2.Controls.Add(pageOption.ControlToShow);
		}

		/// <summary>
		/// Option page description
		/// </summary>
		private class PageOption {
			/// <summary>
			/// Initializes a new instance of the <see cref="T:PageOption"/> class.
			/// </summary>
			/// <param name="title">The title.</param>
			/// <param name="controlToShow">The control to show.</param>
			public PageOption (string title, Control controlToShow) {
				Title = title;
				ControlToShow = controlToShow;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="T:PageOption"/> class.
			/// </summary>
			public PageOption () {
			}

			/// <summary>
			/// Title of the option page
			/// </summary>
			public string Title;

			/// <summary>
			/// Control to show in the right area
			/// </summary>
			public Control ControlToShow;

			/// <summary>
			/// Returns the title of the option page.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
			/// </returns>
			public override string ToString () {
				return Title;
			}
		}

		/// <summary>
		/// Options page
		/// </summary>
		private enum AvailablePageOption {
			/// <summary>
			/// Main options - Options g�n�rales
			/// </summary>
			General,
			/// <summary>
			/// Extensions
			/// </summary>
			Extensions,
			/// <summary>
			/// Directories to watch - R�pertoires � surveiller
			/// </summary>
			Directories,
			/// <summary>
			/// ID3v1/ID3v2 tags
			/// </summary>
			ID3,
			/// <summary>
			/// Database (MySQL config)
			/// </summary>
			Database,
			/// <summary>
			/// SQL Queries - Requ�tes SQL
			/// </summary>
			Queries
		}

		/// <summary>
		/// Gets the page option informations.
		/// </summary>
		/// <param name="aPageOption">An available page option.</param>
		/// <exception cref="T:System.ArgumentException">Throw if aPageOption is not defined</exception>
		/// <returns>Instance of PageOption with correct title and control to show</returns>
		private PageOption GetPageOption (AvailablePageOption aPageOption) {
			PageOption pageOption = new PageOption();
			#region switch d�finissant pageOption.Title et pageOption.ControlToShow
			switch (aPageOption) {
				case AvailablePageOption.Database:
					pageOption.Title = "Serveur MySQL";
					pageOption.ControlToShow = panelConfigDatabase;
					break;

				case AvailablePageOption.Directories:
					pageOption.Title = "Dossiers � surveiller";
					pageOption.ControlToShow = tableLayoutPanelConfigDirectories;
					break;

				case AvailablePageOption.Extensions:
					pageOption.Title = "Extensions audio/vid�o";
					pageOption.ControlToShow = labelNotYetDefinedPanel;
					break;

				case AvailablePageOption.General:
					pageOption.Title = "Options g�n�rales";
					pageOption.ControlToShow = panelConfigMainOptions;
					break;

				case AvailablePageOption.ID3:
					pageOption.Title = "Tags ID3";
					pageOption.ControlToShow = labelNotYetDefinedPanel;
					break;

				case AvailablePageOption.Queries:
					pageOption.Title = "Requ�tes SQL";
					pageOption.ControlToShow = labelNotYetDefinedPanel;
					break;

				default:
					throw new ArgumentException("This option page must be defined in MainForm.GetPageOption(aPageOption) method.");
			}
			#endregion
			return pageOption;
		}

		#endregion

		#region Page Config :: Param�tres MySQL
		/// <summary>
		/// Handles the Click event of the buttonApplyMySQLParameters control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void buttonApplyMySQLParameters_Click (object sender, EventArgs e) {
			#region StringBuilder ConnectionString
			StringBuilder sb = new StringBuilder();
			//Host
			if (textBoxMySQLHost.Text.Length == 0) {
				textBoxMySQLHost.Text = "localhost";
			}
			sb.Append("Server=");
			sb.Append(textBoxMySQLHost.Text);
			//Port
			if (textBoxMySQLPort.Text.Length > 0) {
				//TODO : check if parameter name Port is correct
				sb.Append(";Port=");
				sb.Append(textBoxMySQLPort.Text);
			}
			//Login
			if (textBoxMySQLLogin.Text.Length == 0) {
				textBoxMySQLLogin.Text = "root";
			}
			sb.Append(";Uid=");
			sb.Append(textBoxMySQLLogin.Text);
			//Pass
			if (textBoxMySQLPassword.Text.Length > 0) {
				sb.Append(";Pwd=");
				sb.Append(textBoxMySQLPassword.Text);
			}
			//Database
			sb.Append(";Database=");
			sb.Append(textBoxMySQLDatabase.Text);
			#endregion

			//Ok, our string is built, let's apply it
			textBoxConnectionString.Text = Program.options.ConnectionString = sb.ToString();

			//If database is omitted, user must append the string. Let's notice him
			if (textBoxMySQLDatabase.Text.Length == 0) {
				MessageBox.Show(
					"La cha�ne de connexion se termine actuellement par DataBase=\n\nAjoutez le nom de la base de donn�es contenant l'index.",
					"ConnectionString incompl�te",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
				);
			}
		}

		/// <summary>
		/// Handles the TextChanged event of the textBoxConnectionString control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void textBoxConnectionString_TextChanged (object sender, EventArgs e) {
			TextBox textBox = (TextBox)sender;
			Program.options.ConnectionString = textBox.Text;
			OptionsHasBeenModified = true;
		}

		/// <summary>
		/// Handles the Click event of the buttonProbeConnect control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		private void buttonProbeConnect_Click (object sender, EventArgs e) {
			ProbeMySqlConnectionForm frm = new ProbeMySqlConnectionForm(Program.options.ConnectionString);
			frm.ShowDialog();
		}
		#endregion

	}
}