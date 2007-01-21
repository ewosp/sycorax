/*
 * (c) Sébastien Santoro aka Dereckson, 2006, tous droits réservés
 *
 * Date: 14/07/2006
 * Time: 16:46
 *
 */
namespace Sycorax.ControlCenter {
	partial class MainForm : System.Windows.Forms.Form {
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent () {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabPageDebug = new System.Windows.Forms.TabPage();
            this.textBoxException = new System.Windows.Forms.TextBox();
            this.tabPageConfig = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxOptions = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonRAZOptions = new System.Windows.Forms.Button();
            this.buttonEnregistrerOptions = new System.Windows.Forms.Button();
            this.tableLayoutPanelConfigDirectories = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.textBoxFolderToAdd = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.buttonAddFolder = new System.Windows.Forms.Button();
            this.labelDirectoriesToIndex = new System.Windows.Forms.Label();
            this.buttonDeleteSelectedFolders = new System.Windows.Forms.Button();
            this.checkedListBoxFolders = new System.Windows.Forms.CheckedListBox();
            this.panelConfigDatabase = new System.Windows.Forms.Panel();
            this.tableLayoutPanelConfigDB = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxConfigDB = new System.Windows.Forms.GroupBox();
            this.labelMySQLDatabase = new System.Windows.Forms.Label();
            this.textBoxMySQLDatabase = new System.Windows.Forms.TextBox();
            this.labelMySQLPort = new System.Windows.Forms.Label();
            this.textBoxMySQLPort = new System.Windows.Forms.TextBox();
            this.labelMySQLPassword = new System.Windows.Forms.Label();
            this.textBoxMySQLPassword = new System.Windows.Forms.TextBox();
            this.textBoxMySQLLogin = new System.Windows.Forms.TextBox();
            this.textBoxMySQLHost = new System.Windows.Forms.TextBox();
            this.labelMySQLLogin = new System.Windows.Forms.Label();
            this.labelMySQLHost = new System.Windows.Forms.Label();
            this.buttonApplyMySQLParameters = new System.Windows.Forms.Button();
            this.groupBoxConnectionString = new System.Windows.Forms.GroupBox();
            this.buttonProbeConnect = new System.Windows.Forms.Button();
            this.textBoxConnectionString = new System.Windows.Forms.TextBox();
            this.pictureBoxPoweredByMySql = new System.Windows.Forms.PictureBox();
            this.tabPageRebuildIndex = new System.Windows.Forms.TabPage();
            this.buttonStartRebuildIndex = new System.Windows.Forms.Button();
            this.progressBarRebuild = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPageAutoUpdate = new System.Windows.Forms.TabPage();
            this.panelServiceController = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelInternalAutoUpdateStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.linkInternalAutoUpdateUp = new System.Windows.Forms.LinkLabel();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelAutoUpdateException = new System.Windows.Forms.Label();
            this.labelAutoUpdateHelp1 = new System.Windows.Forms.Label();
            this.linkLabelServicePlayStop = new System.Windows.Forms.LinkLabel();
            this.labelStatutAutoUpdateServiceResult = new System.Windows.Forms.Label();
            this.labelAutoUpdateHelp2 = new System.Windows.Forms.Label();
            this.labelStatutAutoUpdateService = new System.Windows.Forms.Label();
            this.linkLabelServicePauseContinue = new System.Windows.Forms.LinkLabel();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.labelNotYetDefinedPanel = new System.Windows.Forms.Label();
            this.panelConfigMainOptions = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxDebugMode = new System.Windows.Forms.CheckBox();
            this.checkBoxDeleteTunesIfOrphans = new System.Windows.Forms.CheckBox();
            this.tabPageDebug.SuspendLayout();
            this.tabPageConfig.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelConfigDirectories.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.panelConfigDatabase.SuspendLayout();
            this.tableLayoutPanelConfigDB.SuspendLayout();
            this.groupBoxConfigDB.SuspendLayout();
            this.groupBoxConnectionString.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPoweredByMySql)).BeginInit();
            this.tabPageRebuildIndex.SuspendLayout();
            this.tabPageAutoUpdate.SuspendLayout();
            this.panelServiceController.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.panelConfigMainOptions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageDebug
            // 
            this.tabPageDebug.Controls.Add(this.textBoxException);
            this.tabPageDebug.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPageDebug.Location = new System.Drawing.Point(4, 22);
            this.tabPageDebug.Name = "tabPageDebug";
            this.tabPageDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDebug.Size = new System.Drawing.Size(521, 274);
            this.tabPageDebug.TabIndex = 2;
            this.tabPageDebug.Text = "Debug";
            this.tabPageDebug.UseVisualStyleBackColor = true;
            // 
            // textBoxException
            // 
            this.textBoxException.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxException.Location = new System.Drawing.Point(3, 3);
            this.textBoxException.Multiline = true;
            this.textBoxException.Name = "textBoxException";
            this.textBoxException.Size = new System.Drawing.Size(515, 268);
            this.textBoxException.TabIndex = 0;
            // 
            // tabPageConfig
            // 
            this.tabPageConfig.Controls.Add(this.splitContainer1);
            this.tabPageConfig.Location = new System.Drawing.Point(4, 22);
            this.tabPageConfig.Name = "tabPageConfig";
            this.tabPageConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfig.Size = new System.Drawing.Size(577, 543);
            this.tabPageConfig.TabIndex = 4;
            this.tabPageConfig.Text = "Configuration";
            this.tabPageConfig.UseVisualStyleBackColor = true;
            this.tabPageConfig.Enter += new System.EventHandler(this.tabPageConfig_Enter);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxOptions);
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanelConfigDirectories);
            this.splitContainer1.Size = new System.Drawing.Size(571, 537);
            this.splitContainer1.SplitterDistance = 196;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBoxOptions
            // 
            this.listBoxOptions.BackColor = System.Drawing.Color.GreenYellow;
            this.listBoxOptions.Cursor = System.Windows.Forms.Cursors.Default;
            this.listBoxOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxOptions.ForeColor = System.Drawing.Color.SeaGreen;
            this.listBoxOptions.FormattingEnabled = true;
            this.listBoxOptions.ItemHeight = 20;
            this.listBoxOptions.Items.AddRange(new object[] {
            "Options générales",
            "Extensions",
            "Dossiers",
            "Tags ID3",
            "Base de données",
            "Requêtes SQL"});
            this.listBoxOptions.Location = new System.Drawing.Point(0, 0);
            this.listBoxOptions.Name = "listBoxOptions";
            this.listBoxOptions.Size = new System.Drawing.Size(196, 464);
            this.listBoxOptions.TabIndex = 2;
            this.listBoxOptions.SelectedIndexChanged += new System.EventHandler(this.listBoxOptions_SelectedIndexChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.buttonRAZOptions);
            this.flowLayoutPanel1.Controls.Add(this.buttonEnregistrerOptions);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 474);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(196, 63);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // buttonRAZOptions
            // 
            this.buttonRAZOptions.BackColor = System.Drawing.Color.GreenYellow;
            this.buttonRAZOptions.ForeColor = System.Drawing.Color.SeaGreen;
            this.buttonRAZOptions.Location = new System.Drawing.Point(3, 3);
            this.buttonRAZOptions.Name = "buttonRAZOptions";
            this.buttonRAZOptions.Size = new System.Drawing.Size(165, 23);
            this.buttonRAZOptions.TabIndex = 4;
            this.buttonRAZOptions.Text = "Restaurer les options par défaut";
            this.buttonRAZOptions.UseVisualStyleBackColor = false;
            // 
            // buttonEnregistrerOptions
            // 
            this.buttonEnregistrerOptions.BackColor = System.Drawing.Color.GreenYellow;
            this.buttonEnregistrerOptions.Enabled = false;
            this.buttonEnregistrerOptions.ForeColor = System.Drawing.Color.SeaGreen;
            this.buttonEnregistrerOptions.Location = new System.Drawing.Point(3, 32);
            this.buttonEnregistrerOptions.Name = "buttonEnregistrerOptions";
            this.buttonEnregistrerOptions.Size = new System.Drawing.Size(165, 23);
            this.buttonEnregistrerOptions.TabIndex = 3;
            this.buttonEnregistrerOptions.Text = "Enregistrer les préférences";
            this.buttonEnregistrerOptions.UseVisualStyleBackColor = false;
            // 
            // tableLayoutPanelConfigDirectories
            // 
            this.tableLayoutPanelConfigDirectories.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanelConfigDirectories.ColumnCount = 1;
            this.tableLayoutPanelConfigDirectories.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelConfigDirectories.Controls.Add(this.flowLayoutPanel2, 0, 1);
            this.tableLayoutPanelConfigDirectories.Controls.Add(this.labelDirectoriesToIndex, 0, 0);
            this.tableLayoutPanelConfigDirectories.Controls.Add(this.buttonDeleteSelectedFolders, 0, 3);
            this.tableLayoutPanelConfigDirectories.Controls.Add(this.checkedListBoxFolders, 0, 2);
            this.tableLayoutPanelConfigDirectories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelConfigDirectories.ForeColor = System.Drawing.Color.SteelBlue;
            this.tableLayoutPanelConfigDirectories.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelConfigDirectories.Name = "tableLayoutPanelConfigDirectories";
            this.tableLayoutPanelConfigDirectories.RowCount = 5;
            this.tableLayoutPanelConfigDirectories.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelConfigDirectories.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 43F));
            this.tableLayoutPanelConfigDirectories.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 207F));
            this.tableLayoutPanelConfigDirectories.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanelConfigDirectories.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelConfigDirectories.Size = new System.Drawing.Size(371, 537);
            this.tableLayoutPanelConfigDirectories.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.textBoxFolderToAdd);
            this.flowLayoutPanel2.Controls.Add(this.buttonBrowse);
            this.flowLayoutPanel2.Controls.Add(this.buttonAddFolder);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 23);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(365, 37);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // textBoxFolderToAdd
            // 
            this.textBoxFolderToAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBoxFolderToAdd.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBoxFolderToAdd.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.textBoxFolderToAdd.Location = new System.Drawing.Point(3, 4);
            this.textBoxFolderToAdd.Name = "textBoxFolderToAdd";
            this.textBoxFolderToAdd.Size = new System.Drawing.Size(246, 20);
            this.textBoxFolderToAdd.TabIndex = 3;
            this.textBoxFolderToAdd.TextChanged += new System.EventHandler(this.textBoxFolderToAdd_TextChanged);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonBrowse.Location = new System.Drawing.Point(255, 3);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(74, 22);
            this.buttonBrowse.TabIndex = 3;
            this.buttonBrowse.Text = "Parcourir ...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // buttonAddFolder
            // 
            this.buttonAddFolder.Enabled = false;
            this.buttonAddFolder.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddFolder.Image")));
            this.buttonAddFolder.Location = new System.Drawing.Point(335, 3);
            this.buttonAddFolder.Name = "buttonAddFolder";
            this.buttonAddFolder.Size = new System.Drawing.Size(23, 23);
            this.buttonAddFolder.TabIndex = 5;
            this.buttonAddFolder.UseVisualStyleBackColor = true;
            this.buttonAddFolder.Click += new System.EventHandler(this.buttonAddFolder_Click);
            // 
            // labelDirectoriesToIndex
            // 
            this.labelDirectoriesToIndex.AutoSize = true;
            this.labelDirectoriesToIndex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDirectoriesToIndex.Location = new System.Drawing.Point(3, 0);
            this.labelDirectoriesToIndex.Name = "labelDirectoriesToIndex";
            this.labelDirectoriesToIndex.Size = new System.Drawing.Size(119, 13);
            this.labelDirectoriesToIndex.TabIndex = 2;
            this.labelDirectoriesToIndex.Text = "Dossiers à indexer :";
            // 
            // buttonDeleteSelectedFolders
            // 
            this.buttonDeleteSelectedFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonDeleteSelectedFolders.Enabled = false;
            this.buttonDeleteSelectedFolders.Location = new System.Drawing.Point(3, 273);
            this.buttonDeleteSelectedFolders.Name = "buttonDeleteSelectedFolders";
            this.buttonDeleteSelectedFolders.Size = new System.Drawing.Size(365, 19);
            this.buttonDeleteSelectedFolders.TabIndex = 4;
            this.buttonDeleteSelectedFolders.Text = "Supprimer les dossiers sélectionnés";
            this.buttonDeleteSelectedFolders.UseVisualStyleBackColor = true;
            this.buttonDeleteSelectedFolders.Click += new System.EventHandler(this.buttonDeleteSelectedFolders_Click);
            // 
            // checkedListBoxFolders
            // 
            this.checkedListBoxFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxFolders.FormattingEnabled = true;
            this.checkedListBoxFolders.Location = new System.Drawing.Point(3, 66);
            this.checkedListBoxFolders.Name = "checkedListBoxFolders";
            this.checkedListBoxFolders.Size = new System.Drawing.Size(365, 199);
            this.checkedListBoxFolders.TabIndex = 6;
            this.checkedListBoxFolders.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxFolders_SelectedIndexChanged);
            // 
            // panelConfigDatabase
            // 
            this.panelConfigDatabase.Controls.Add(this.tableLayoutPanelConfigDB);
            this.panelConfigDatabase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConfigDatabase.Location = new System.Drawing.Point(0, 0);
            this.panelConfigDatabase.Name = "panelConfigDatabase";
            this.panelConfigDatabase.Size = new System.Drawing.Size(405, 775);
            this.panelConfigDatabase.TabIndex = 0;
            // 
            // tableLayoutPanelConfigDB
            // 
            this.tableLayoutPanelConfigDB.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanelConfigDB.ColumnCount = 1;
            this.tableLayoutPanelConfigDB.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelConfigDB.Controls.Add(this.groupBoxConfigDB, 0, 1);
            this.tableLayoutPanelConfigDB.Controls.Add(this.groupBoxConnectionString, 0, 2);
            this.tableLayoutPanelConfigDB.Controls.Add(this.pictureBoxPoweredByMySql, 0, 0);
            this.tableLayoutPanelConfigDB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelConfigDB.ForeColor = System.Drawing.Color.SteelBlue;
            this.tableLayoutPanelConfigDB.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelConfigDB.Name = "tableLayoutPanelConfigDB";
            this.tableLayoutPanelConfigDB.RowCount = 4;
            this.tableLayoutPanelConfigDB.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanelConfigDB.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanelConfigDB.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanelConfigDB.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelConfigDB.Size = new System.Drawing.Size(405, 775);
            this.tableLayoutPanelConfigDB.TabIndex = 3;
            // 
            // groupBoxConfigDB
            // 
            this.groupBoxConfigDB.Controls.Add(this.labelMySQLDatabase);
            this.groupBoxConfigDB.Controls.Add(this.textBoxMySQLDatabase);
            this.groupBoxConfigDB.Controls.Add(this.labelMySQLPort);
            this.groupBoxConfigDB.Controls.Add(this.textBoxMySQLPort);
            this.groupBoxConfigDB.Controls.Add(this.labelMySQLPassword);
            this.groupBoxConfigDB.Controls.Add(this.textBoxMySQLPassword);
            this.groupBoxConfigDB.Controls.Add(this.textBoxMySQLLogin);
            this.groupBoxConfigDB.Controls.Add(this.textBoxMySQLHost);
            this.groupBoxConfigDB.Controls.Add(this.labelMySQLLogin);
            this.groupBoxConfigDB.Controls.Add(this.labelMySQLHost);
            this.groupBoxConfigDB.Controls.Add(this.buttonApplyMySQLParameters);
            this.groupBoxConfigDB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxConfigDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxConfigDB.Location = new System.Drawing.Point(3, 73);
            this.groupBoxConfigDB.Name = "groupBoxConfigDB";
            this.groupBoxConfigDB.Size = new System.Drawing.Size(399, 194);
            this.groupBoxConfigDB.TabIndex = 2;
            this.groupBoxConfigDB.TabStop = false;
            this.groupBoxConfigDB.Text = "Nouveaux paramètres de connexion";
            // 
            // labelMySQLDatabase
            // 
            this.labelMySQLDatabase.AutoSize = true;
            this.labelMySQLDatabase.Location = new System.Drawing.Point(15, 133);
            this.labelMySQLDatabase.Name = "labelMySQLDatabase";
            this.labelMySQLDatabase.Size = new System.Drawing.Size(59, 13);
            this.labelMySQLDatabase.TabIndex = 12;
            this.labelMySQLDatabase.Text = "Database :";
            // 
            // textBoxMySQLDatabase
            // 
            this.textBoxMySQLDatabase.Location = new System.Drawing.Point(81, 130);
            this.textBoxMySQLDatabase.Name = "textBoxMySQLDatabase";
            this.textBoxMySQLDatabase.Size = new System.Drawing.Size(190, 20);
            this.textBoxMySQLDatabase.TabIndex = 8;
            // 
            // labelMySQLPort
            // 
            this.labelMySQLPort.AutoSize = true;
            this.labelMySQLPort.Location = new System.Drawing.Point(15, 55);
            this.labelMySQLPort.Name = "labelMySQLPort";
            this.labelMySQLPort.Size = new System.Drawing.Size(32, 13);
            this.labelMySQLPort.TabIndex = 10;
            this.labelMySQLPort.Text = "Port :";
            // 
            // textBoxMySQLPort
            // 
            this.textBoxMySQLPort.Location = new System.Drawing.Point(81, 52);
            this.textBoxMySQLPort.Name = "textBoxMySQLPort";
            this.textBoxMySQLPort.Size = new System.Drawing.Size(190, 20);
            this.textBoxMySQLPort.TabIndex = 5;
            // 
            // labelMySQLPassword
            // 
            this.labelMySQLPassword.AutoSize = true;
            this.labelMySQLPassword.Location = new System.Drawing.Point(15, 107);
            this.labelMySQLPassword.Name = "labelMySQLPassword";
            this.labelMySQLPassword.Size = new System.Drawing.Size(59, 13);
            this.labelMySQLPassword.TabIndex = 8;
            this.labelMySQLPassword.Text = "Password :";
            // 
            // textBoxMySQLPassword
            // 
            this.textBoxMySQLPassword.Location = new System.Drawing.Point(81, 104);
            this.textBoxMySQLPassword.Name = "textBoxMySQLPassword";
            this.textBoxMySQLPassword.Size = new System.Drawing.Size(190, 20);
            this.textBoxMySQLPassword.TabIndex = 7;
            // 
            // textBoxMySQLLogin
            // 
            this.textBoxMySQLLogin.Location = new System.Drawing.Point(81, 78);
            this.textBoxMySQLLogin.Name = "textBoxMySQLLogin";
            this.textBoxMySQLLogin.Size = new System.Drawing.Size(190, 20);
            this.textBoxMySQLLogin.TabIndex = 6;
            // 
            // textBoxMySQLHost
            // 
            this.textBoxMySQLHost.Location = new System.Drawing.Point(81, 26);
            this.textBoxMySQLHost.Name = "textBoxMySQLHost";
            this.textBoxMySQLHost.Size = new System.Drawing.Size(190, 20);
            this.textBoxMySQLHost.TabIndex = 4;
            // 
            // labelMySQLLogin
            // 
            this.labelMySQLLogin.AutoSize = true;
            this.labelMySQLLogin.Location = new System.Drawing.Point(15, 81);
            this.labelMySQLLogin.Name = "labelMySQLLogin";
            this.labelMySQLLogin.Size = new System.Drawing.Size(39, 13);
            this.labelMySQLLogin.TabIndex = 2;
            this.labelMySQLLogin.Text = "Login :";
            // 
            // labelMySQLHost
            // 
            this.labelMySQLHost.AutoSize = true;
            this.labelMySQLHost.Location = new System.Drawing.Point(15, 29);
            this.labelMySQLHost.Name = "labelMySQLHost";
            this.labelMySQLHost.Size = new System.Drawing.Size(50, 13);
            this.labelMySQLHost.TabIndex = 1;
            this.labelMySQLHost.Text = "Serveur :";
            // 
            // buttonApplyMySQLParameters
            // 
            this.buttonApplyMySQLParameters.BackColor = System.Drawing.Color.White;
            this.buttonApplyMySQLParameters.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonApplyMySQLParameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonApplyMySQLParameters.ForeColor = System.Drawing.Color.SteelBlue;
            this.buttonApplyMySQLParameters.Location = new System.Drawing.Point(3, 168);
            this.buttonApplyMySQLParameters.Name = "buttonApplyMySQLParameters";
            this.buttonApplyMySQLParameters.Size = new System.Drawing.Size(393, 23);
            this.buttonApplyMySQLParameters.TabIndex = 9;
            this.buttonApplyMySQLParameters.Text = "Prendre en compte ces nouveaux paramètres";
            this.buttonApplyMySQLParameters.UseVisualStyleBackColor = false;
            this.buttonApplyMySQLParameters.Click += new System.EventHandler(this.buttonApplyMySQLParameters_Click);
            // 
            // groupBoxConnectionString
            // 
            this.groupBoxConnectionString.Controls.Add(this.buttonProbeConnect);
            this.groupBoxConnectionString.Controls.Add(this.textBoxConnectionString);
            this.groupBoxConnectionString.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxConnectionString.Location = new System.Drawing.Point(3, 273);
            this.groupBoxConnectionString.Name = "groupBoxConnectionString";
            this.groupBoxConnectionString.Size = new System.Drawing.Size(399, 74);
            this.groupBoxConnectionString.TabIndex = 3;
            this.groupBoxConnectionString.TabStop = false;
            this.groupBoxConnectionString.Text = "ConnectionString";
            // 
            // buttonProbeConnect
            // 
            this.buttonProbeConnect.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonProbeConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonProbeConnect.Location = new System.Drawing.Point(3, 48);
            this.buttonProbeConnect.Name = "buttonProbeConnect";
            this.buttonProbeConnect.Size = new System.Drawing.Size(393, 23);
            this.buttonProbeConnect.TabIndex = 11;
            this.buttonProbeConnect.Text = "Tester la connexion";
            this.buttonProbeConnect.UseVisualStyleBackColor = true;
            this.buttonProbeConnect.Click += new System.EventHandler(this.buttonProbeConnect_Click);
            // 
            // textBoxConnectionString
            // 
            this.textBoxConnectionString.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxConnectionString.Location = new System.Drawing.Point(3, 16);
            this.textBoxConnectionString.Multiline = true;
            this.textBoxConnectionString.Name = "textBoxConnectionString";
            this.textBoxConnectionString.Size = new System.Drawing.Size(393, 55);
            this.textBoxConnectionString.TabIndex = 10;
            this.textBoxConnectionString.TextChanged += new System.EventHandler(this.textBoxConnectionString_TextChanged);
            // 
            // pictureBoxPoweredByMySql
            // 
            this.pictureBoxPoweredByMySql.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxPoweredByMySql.Image = global::Sycorax.ControlCenter.Properties.Resources.poweredByMysql125x64;
            this.pictureBoxPoweredByMySql.Location = new System.Drawing.Point(277, 3);
            this.pictureBoxPoweredByMySql.Name = "pictureBoxPoweredByMySql";
            this.pictureBoxPoweredByMySql.Size = new System.Drawing.Size(125, 64);
            this.pictureBoxPoweredByMySql.TabIndex = 4;
            this.pictureBoxPoweredByMySql.TabStop = false;
            // 
            // tabPageRebuildIndex
            // 
            this.tabPageRebuildIndex.Controls.Add(this.buttonStartRebuildIndex);
            this.tabPageRebuildIndex.Controls.Add(this.progressBarRebuild);
            this.tabPageRebuildIndex.Controls.Add(this.label2);
            this.tabPageRebuildIndex.Location = new System.Drawing.Point(4, 22);
            this.tabPageRebuildIndex.Name = "tabPageRebuildIndex";
            this.tabPageRebuildIndex.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRebuildIndex.Size = new System.Drawing.Size(577, 543);
            this.tabPageRebuildIndex.TabIndex = 3;
            this.tabPageRebuildIndex.Text = "Regénérer l\'index";
            this.tabPageRebuildIndex.UseVisualStyleBackColor = true;
            // 
            // buttonStartRebuildIndex
            // 
            this.buttonStartRebuildIndex.Location = new System.Drawing.Point(11, 101);
            this.buttonStartRebuildIndex.Name = "buttonStartRebuildIndex";
            this.buttonStartRebuildIndex.Size = new System.Drawing.Size(75, 26);
            this.buttonStartRebuildIndex.TabIndex = 3;
            this.buttonStartRebuildIndex.Text = "Démarrer";
            this.buttonStartRebuildIndex.UseVisualStyleBackColor = true;
            this.buttonStartRebuildIndex.Click += new System.EventHandler(this.buttonStartRebuildIndex_Click);
            // 
            // progressBarRebuild
            // 
            this.progressBarRebuild.BackColor = System.Drawing.Color.White;
            this.progressBarRebuild.ForeColor = System.Drawing.Color.LawnGreen;
            this.progressBarRebuild.Location = new System.Drawing.Point(92, 101);
            this.progressBarRebuild.Name = "progressBarRebuild";
            this.progressBarRebuild.Size = new System.Drawing.Size(475, 26);
            this.progressBarRebuild.TabIndex = 2;
            this.progressBarRebuild.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(559, 78);
            this.label2.TabIndex = 1;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // tabPageAutoUpdate
            // 
            this.tabPageAutoUpdate.Controls.Add(this.panelServiceController);
            this.tabPageAutoUpdate.Location = new System.Drawing.Point(4, 22);
            this.tabPageAutoUpdate.Name = "tabPageAutoUpdate";
            this.tabPageAutoUpdate.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAutoUpdate.Size = new System.Drawing.Size(577, 543);
            this.tabPageAutoUpdate.TabIndex = 1;
            this.tabPageAutoUpdate.Text = "Auto Update";
            this.tabPageAutoUpdate.UseVisualStyleBackColor = true;
            this.tabPageAutoUpdate.Enter += new System.EventHandler(this.tabPageAutoUpdate_Enter);
            // 
            // panelServiceController
            // 
            this.panelServiceController.Controls.Add(this.groupBox3);
            this.panelServiceController.Controls.Add(this.groupBox2);
            this.panelServiceController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelServiceController.Location = new System.Drawing.Point(3, 3);
            this.panelServiceController.Name = "panelServiceController";
            this.panelServiceController.Size = new System.Drawing.Size(571, 537);
            this.panelServiceController.TabIndex = 13;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.labelInternalAutoUpdateStatus);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.linkInternalAutoUpdateUp);
            this.groupBox3.Controls.Add(this.textBoxLog);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 89);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(571, 448);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Bypass service (DEBUG)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(303, 26);
            this.label4.TabIndex = 10;
            this.label4.Text = "En lieu et place du service, vous pouvez lancer l\'auto update\r\ndirectement depuis" +
                " Sycorax Conrol Center à des fins de debug.";
            // 
            // labelInternalAutoUpdateStatus
            // 
            this.labelInternalAutoUpdateStatus.AutoSize = true;
            this.labelInternalAutoUpdateStatus.Location = new System.Drawing.Point(376, 12);
            this.labelInternalAutoUpdateStatus.Name = "labelInternalAutoUpdateStatus";
            this.labelInternalAutoUpdateStatus.Size = new System.Drawing.Size(21, 13);
            this.labelInternalAutoUpdateStatus.TabIndex = 9;
            this.labelInternalAutoUpdateStatus.Text = "Off";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(342, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Etat :";
            // 
            // linkInternalAutoUpdateUp
            // 
            this.linkInternalAutoUpdateUp.AutoSize = true;
            this.linkInternalAutoUpdateUp.Location = new System.Drawing.Point(376, 31);
            this.linkInternalAutoUpdateUp.Name = "linkInternalAutoUpdateUp";
            this.linkInternalAutoUpdateUp.Size = new System.Drawing.Size(50, 13);
            this.linkInternalAutoUpdateUp.TabIndex = 6;
            this.linkInternalAutoUpdateUp.TabStop = true;
            this.linkInternalAutoUpdateUp.Text = "Démarrer";
            this.linkInternalAutoUpdateUp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkInternalAutoUpdateUp_LinkClicked);
            // 
            // textBoxLog
            // 
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.textBoxLog.Location = new System.Drawing.Point(3, 70);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.Size = new System.Drawing.Size(565, 375);
            this.textBoxLog.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelAutoUpdateException);
            this.groupBox2.Controls.Add(this.labelAutoUpdateHelp1);
            this.groupBox2.Controls.Add(this.linkLabelServicePlayStop);
            this.groupBox2.Controls.Add(this.labelStatutAutoUpdateServiceResult);
            this.groupBox2.Controls.Add(this.labelAutoUpdateHelp2);
            this.groupBox2.Controls.Add(this.labelStatutAutoUpdateService);
            this.groupBox2.Controls.Add(this.linkLabelServicePauseContinue);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(571, 89);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Service";
            // 
            // labelAutoUpdateException
            // 
            this.labelAutoUpdateException.AutoSize = true;
            this.labelAutoUpdateException.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAutoUpdateException.ForeColor = System.Drawing.Color.Maroon;
            this.labelAutoUpdateException.Location = new System.Drawing.Point(6, 55);
            this.labelAutoUpdateException.Name = "labelAutoUpdateException";
            this.labelAutoUpdateException.Size = new System.Drawing.Size(0, 13);
            this.labelAutoUpdateException.TabIndex = 11;
            // 
            // labelAutoUpdateHelp1
            // 
            this.labelAutoUpdateHelp1.AutoSize = true;
            this.labelAutoUpdateHelp1.Location = new System.Drawing.Point(6, 16);
            this.labelAutoUpdateHelp1.Name = "labelAutoUpdateHelp1";
            this.labelAutoUpdateHelp1.Size = new System.Drawing.Size(321, 13);
            this.labelAutoUpdateHelp1.TabIndex = 8;
            this.labelAutoUpdateHelp1.Text = "Sycorax Auto Update est un service indexant en continu vos MP3.";
            // 
            // linkLabelServicePlayStop
            // 
            this.linkLabelServicePlayStop.AutoSize = true;
            this.linkLabelServicePlayStop.Location = new System.Drawing.Point(376, 34);
            this.linkLabelServicePlayStop.Name = "linkLabelServicePlayStop";
            this.linkLabelServicePlayStop.Size = new System.Drawing.Size(50, 13);
            this.linkLabelServicePlayStop.TabIndex = 4;
            this.linkLabelServicePlayStop.TabStop = true;
            this.linkLabelServicePlayStop.Text = "Démarrer";
            this.linkLabelServicePlayStop.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelServicePlayStop_LinkClicked);
            // 
            // labelStatutAutoUpdateServiceResult
            // 
            this.labelStatutAutoUpdateServiceResult.AutoSize = true;
            this.labelStatutAutoUpdateServiceResult.Location = new System.Drawing.Point(376, 16);
            this.labelStatutAutoUpdateServiceResult.Name = "labelStatutAutoUpdateServiceResult";
            this.labelStatutAutoUpdateServiceResult.Size = new System.Drawing.Size(45, 13);
            this.labelStatutAutoUpdateServiceResult.TabIndex = 3;
            this.labelStatutAutoUpdateServiceResult.Text = "inconnu";
            // 
            // labelAutoUpdateHelp2
            // 
            this.labelAutoUpdateHelp2.AutoSize = true;
            this.labelAutoUpdateHelp2.Location = new System.Drawing.Point(6, 34);
            this.labelAutoUpdateHelp2.Name = "labelAutoUpdateHelp2";
            this.labelAutoUpdateHelp2.Size = new System.Drawing.Size(280, 13);
            this.labelAutoUpdateHelp2.TabIndex = 1;
            this.labelAutoUpdateHelp2.Text = "Votre base de données est ainsi constamment mise à jour.";
            // 
            // labelStatutAutoUpdateService
            // 
            this.labelStatutAutoUpdateService.AutoSize = true;
            this.labelStatutAutoUpdateService.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatutAutoUpdateService.Location = new System.Drawing.Point(342, 16);
            this.labelStatutAutoUpdateService.Name = "labelStatutAutoUpdateService";
            this.labelStatutAutoUpdateService.Size = new System.Drawing.Size(38, 13);
            this.labelStatutAutoUpdateService.TabIndex = 2;
            this.labelStatutAutoUpdateService.Text = "Etat :";
            // 
            // linkLabelServicePauseContinue
            // 
            this.linkLabelServicePauseContinue.AutoSize = true;
            this.linkLabelServicePauseContinue.Location = new System.Drawing.Point(376, 52);
            this.linkLabelServicePauseContinue.Name = "linkLabelServicePauseContinue";
            this.linkLabelServicePauseContinue.Size = new System.Drawing.Size(37, 13);
            this.linkLabelServicePauseContinue.TabIndex = 5;
            this.linkLabelServicePauseContinue.TabStop = true;
            this.linkLabelServicePauseContinue.Text = "Pause";
            this.linkLabelServicePauseContinue.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelServicePause_LinkClicked);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageAutoUpdate);
            this.tabControlMain.Controls.Add(this.tabPageRebuildIndex);
            this.tabControlMain.Controls.Add(this.tabPageConfig);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(585, 569);
            this.tabControlMain.TabIndex = 0;
            // 
            // labelNotYetDefinedPanel
            // 
            this.labelNotYetDefinedPanel.AutoSize = true;
            this.labelNotYetDefinedPanel.Location = new System.Drawing.Point(16, 21);
            this.labelNotYetDefinedPanel.Name = "labelNotYetDefinedPanel";
            this.labelNotYetDefinedPanel.Size = new System.Drawing.Size(177, 13);
            this.labelNotYetDefinedPanel.TabIndex = 0;
            this.labelNotYetDefinedPanel.Text = "Ce panneau n\'est pas encore défini.";
            // 
            // panelConfigMainOptions
            // 
            this.panelConfigMainOptions.Controls.Add(this.groupBox1);
            this.panelConfigMainOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConfigMainOptions.Location = new System.Drawing.Point(0, 0);
            this.panelConfigMainOptions.Name = "panelConfigMainOptions";
            this.panelConfigMainOptions.Size = new System.Drawing.Size(405, 775);
            this.panelConfigMainOptions.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxDebugMode);
            this.groupBox1.Controls.Add(this.checkBoxDeleteTunesIfOrphans);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(405, 78);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options générales";
            // 
            // checkBoxDebugMode
            // 
            this.checkBoxDebugMode.AutoSize = true;
            this.checkBoxDebugMode.Location = new System.Drawing.Point(16, 46);
            this.checkBoxDebugMode.Name = "checkBoxDebugMode";
            this.checkBoxDebugMode.Size = new System.Drawing.Size(282, 17);
            this.checkBoxDebugMode.TabIndex = 9;
            this.checkBoxDebugMode.Text = "Activer les fonctionnalités du mode debug de Sycorax.";
            this.checkBoxDebugMode.UseVisualStyleBackColor = true;
            // 
            // checkBoxDeleteTunesIfOrphans
            // 
            this.checkBoxDeleteTunesIfOrphans.AutoSize = true;
            this.checkBoxDeleteTunesIfOrphans.Location = new System.Drawing.Point(16, 24);
            this.checkBoxDeleteTunesIfOrphans.Name = "checkBoxDeleteTunesIfOrphans";
            this.checkBoxDeleteTunesIfOrphans.Size = new System.Drawing.Size(339, 17);
            this.checkBoxDeleteTunesIfOrphans.TabIndex = 8;
            this.checkBoxDeleteTunesIfOrphans.Text = "Supprimer les morceaux de la base de données s\'ils sont orphelins.";
            this.checkBoxDeleteTunesIfOrphans.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 569);
            this.Controls.Add(this.tabControlMain);
            this.Name = "MainForm";
            this.Text = "Sycorax Control Center";
            this.tabPageDebug.ResumeLayout(false);
            this.tabPageDebug.PerformLayout();
            this.tabPageConfig.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanelConfigDirectories.ResumeLayout(false);
            this.tableLayoutPanelConfigDirectories.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.panelConfigDatabase.ResumeLayout(false);
            this.tableLayoutPanelConfigDB.ResumeLayout(false);
            this.groupBoxConfigDB.ResumeLayout(false);
            this.groupBoxConfigDB.PerformLayout();
            this.groupBoxConnectionString.ResumeLayout(false);
            this.groupBoxConnectionString.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPoweredByMySql)).EndInit();
            this.tabPageRebuildIndex.ResumeLayout(false);
            this.tabPageRebuildIndex.PerformLayout();
            this.tabPageAutoUpdate.ResumeLayout(false);
            this.panelServiceController.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.panelConfigMainOptions.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.TabPage tabPageDebug;
		private System.Windows.Forms.TextBox textBoxException;
		private System.Windows.Forms.TabPage tabPageConfig;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button buttonRAZOptions;
		private System.Windows.Forms.Button buttonEnregistrerOptions;
		private System.Windows.Forms.Panel panelConfigDatabase;
		private System.Windows.Forms.TabPage tabPageRebuildIndex;
		private System.Windows.Forms.Button buttonStartRebuildIndex;
		private System.Windows.Forms.ProgressBar progressBarRebuild;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TabPage tabPageAutoUpdate;
		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.ListBox listBoxOptions;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelConfigDB;
		private System.Windows.Forms.GroupBox groupBoxConfigDB;
		private System.Windows.Forms.Button buttonApplyMySQLParameters;
		private System.Windows.Forms.GroupBox groupBoxConnectionString;
		private System.Windows.Forms.TextBox textBoxConnectionString;
		private System.Windows.Forms.Label labelMySQLPassword;
		private System.Windows.Forms.TextBox textBoxMySQLPassword;
		private System.Windows.Forms.TextBox textBoxMySQLLogin;
		private System.Windows.Forms.TextBox textBoxMySQLHost;
		private System.Windows.Forms.Label labelMySQLLogin;
		private System.Windows.Forms.Label labelMySQLHost;
		private System.Windows.Forms.Label labelMySQLPort;
		private System.Windows.Forms.TextBox textBoxMySQLPort;
		private System.Windows.Forms.Label labelMySQLDatabase;
		private System.Windows.Forms.TextBox textBoxMySQLDatabase;
		private System.Windows.Forms.PictureBox pictureBoxPoweredByMySql;
		private System.Windows.Forms.Button buttonProbeConnect;
		private System.Windows.Forms.Label labelNotYetDefinedPanel;
		private System.Windows.Forms.Panel panelConfigMainOptions;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkBoxDebugMode;
		private System.Windows.Forms.CheckBox checkBoxDeleteTunesIfOrphans;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelConfigDirectories;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
		private System.Windows.Forms.TextBox textBoxFolderToAdd;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.Label labelDirectoriesToIndex;
		private System.Windows.Forms.Button buttonAddFolder;
		private System.Windows.Forms.Button buttonDeleteSelectedFolders;
		private System.Windows.Forms.CheckedListBox checkedListBoxFolders;
		private System.Windows.Forms.Panel panelServiceController;
		private System.Windows.Forms.Label labelAutoUpdateHelp1;
		private System.Windows.Forms.Label labelAutoUpdateHelp2;
		private System.Windows.Forms.LinkLabel linkLabelServicePauseContinue;
		private System.Windows.Forms.Label labelStatutAutoUpdateService;
		private System.Windows.Forms.Label labelStatutAutoUpdateServiceResult;
		private System.Windows.Forms.LinkLabel linkLabelServicePlayStop;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label labelAutoUpdateException;
		private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.LinkLabel linkInternalAutoUpdateUp;
		private System.Windows.Forms.TextBox textBoxLog;
		private System.Windows.Forms.Label labelInternalAutoUpdateStatus;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
	}
}