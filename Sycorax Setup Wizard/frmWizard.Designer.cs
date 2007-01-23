namespace Sycorax.SetupWizard {
    partial class frmWizard {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWizard));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.wizardForm1 = new UtilityLibrary.Wizards.WizardForm();
            this.wizardWelcomePage1 = new UtilityLibrary.Wizards.WizardWelcomePage();
            this.wizardPageBase1 = new UtilityLibrary.Wizards.WizardPageBase();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonMySQLJustFillParameters = new System.Windows.Forms.RadioButton();
            this.radioButtonMySQLCreateTable = new System.Windows.Forms.RadioButton();
            this.radioButtonMySQLCreateDatabase = new System.Windows.Forms.RadioButton();
            this.radioButtonMySQLNotYetInstalled = new System.Windows.Forms.RadioButton();
            this.wizardPageBase2 = new UtilityLibrary.Wizards.WizardPageBase();
            this.labelMySQLServiceCheckResult = new System.Windows.Forms.Label();
            this.buttonMySQLServiceCheck = new System.Windows.Forms.Button();
            this.textBoxMySQLServiceName = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.linkLabelMySQLDownload = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.wizardFinalPage1 = new UtilityLibrary.Wizards.WizardFinalPage();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.wizardPageBase3 = new UtilityLibrary.Wizards.WizardPageBase();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxUid = new System.Windows.Forms.TextBox();
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.buttonMySQLConnect1 = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.wizardPageBase1.SuspendLayout();
            this.wizardPageBase2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.wizardPageBase3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 295F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.wizardForm1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(942, 449);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // wizardForm1
            // 
            this.wizardForm1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardForm1.Location = new System.Drawing.Point(298, 3);
            this.wizardForm1.Name = "wizardForm1";
            this.wizardForm1.PageIndex = 4;
            this.wizardForm1.Pages.AddRange(new UtilityLibrary.Wizards.WizardPageBase[] {
            this.wizardWelcomePage1,
            this.wizardPageBase1,
            this.wizardPageBase2,
            this.wizardFinalPage1,
            this.wizardPageBase3});
            this.wizardForm1.ShowCancelConfirm = false;
            this.wizardForm1.Size = new System.Drawing.Size(641, 443);
            this.wizardForm1.TabIndex = 1;
            // 
            // wizardWelcomePage1
            // 
            this.wizardWelcomePage1.BackColor = System.Drawing.Color.White;
            this.wizardWelcomePage1.Description = "Cet assistant va vous permettre de configurer Sycorax pas à pas. Si vous êtes déj" +
                "à expérimenté, vous pouvez aussi préférer utiliser directement Sycorax Control C" +
                "enter.";
            this.wizardWelcomePage1.Description2 = resources.GetString("wizardWelcomePage1.Description2");
            this.wizardWelcomePage1.HeaderImage = global::Sycorax.SetupWizard.Properties.Resources.Puzzle;
            this.wizardWelcomePage1.Index = 0;
            this.wizardWelcomePage1.Name = "wizardWelcomePage1";
            this.wizardWelcomePage1.Size = new System.Drawing.Size(641, 396);
            this.wizardWelcomePage1.TabIndex = 0;
            this.wizardWelcomePage1.Title = "Bienvenue dans Sycorax";
            this.wizardWelcomePage1.WizardPageParent = this.wizardForm1;
            // 
            // wizardPageBase1
            // 
            this.wizardPageBase1.Controls.Add(this.label4);
            this.wizardPageBase1.Controls.Add(this.label3);
            this.wizardPageBase1.Controls.Add(this.label2);
            this.wizardPageBase1.Controls.Add(this.label1);
            this.wizardPageBase1.Controls.Add(this.radioButtonMySQLJustFillParameters);
            this.wizardPageBase1.Controls.Add(this.radioButtonMySQLCreateTable);
            this.wizardPageBase1.Controls.Add(this.radioButtonMySQLCreateDatabase);
            this.wizardPageBase1.Controls.Add(this.radioButtonMySQLNotYetInstalled);
            this.wizardPageBase1.Description = "Quelles tâches MySQL souhaitez-vous confiez à l\'assistant ?";
            this.wizardPageBase1.Index = 1;
            this.wizardPageBase1.Name = "wizardPageBase1";
            this.wizardPageBase1.Size = new System.Drawing.Size(625, 332);
            this.wizardPageBase1.TabIndex = 0;
            this.wizardPageBase1.Title = "Connexion MySQL";
            this.wizardPageBase1.WizardPageParent = this.wizardForm1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 240);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(345, 26);
            this.label4.TabIndex = 7;
            this.label4.Text = "Conseil : si vous êtes connecté en permanence à InterNet, rien ne\r\nvous empêche d" +
                "\'utiliser comme serveur MySQL celui de votre site Web.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(25, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(284, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Où en êtes-vous dans votre installation MySQL ?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(419, 39);
            this.label2.TabIndex = 5;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(455, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Sycorax stocke dans une base de données MySQL l\'index de vos fichiers MP3.";
            // 
            // radioButtonMySQLJustFillParameters
            // 
            this.radioButtonMySQLJustFillParameters.AutoSize = true;
            this.radioButtonMySQLJustFillParameters.Location = new System.Drawing.Point(43, 207);
            this.radioButtonMySQLJustFillParameters.Name = "radioButtonMySQLJustFillParameters";
            this.radioButtonMySQLJustFillParameters.Size = new System.Drawing.Size(188, 17);
            this.radioButtonMySQLJustFillParameters.TabIndex = 3;
            this.radioButtonMySQLJustFillParameters.Text = "Tout est prêt, y compris les tables !";
            this.radioButtonMySQLJustFillParameters.UseVisualStyleBackColor = true;
            this.radioButtonMySQLJustFillParameters.CheckedChanged += new System.EventHandler(this.radioButtonMySQL_CheckedChanged);
            // 
            // radioButtonMySQLCreateTable
            // 
            this.radioButtonMySQLCreateTable.AutoSize = true;
            this.radioButtonMySQLCreateTable.Location = new System.Drawing.Point(43, 183);
            this.radioButtonMySQLCreateTable.Name = "radioButtonMySQLCreateTable";
            this.radioButtonMySQLCreateTable.Size = new System.Drawing.Size(403, 17);
            this.radioButtonMySQLCreateTable.TabIndex = 2;
            this.radioButtonMySQLCreateTable.Text = "J\'ai un login, un mot de passe et une base de données déjà créée pour Sycorax.";
            this.radioButtonMySQLCreateTable.UseVisualStyleBackColor = true;
            this.radioButtonMySQLCreateTable.CheckedChanged += new System.EventHandler(this.radioButtonMySQL_CheckedChanged);
            // 
            // radioButtonMySQLCreateDatabase
            // 
            this.radioButtonMySQLCreateDatabase.AutoSize = true;
            this.radioButtonMySQLCreateDatabase.Location = new System.Drawing.Point(43, 159);
            this.radioButtonMySQLCreateDatabase.Name = "radioButtonMySQLCreateDatabase";
            this.radioButtonMySQLCreateDatabase.Size = new System.Drawing.Size(381, 17);
            this.radioButtonMySQLCreateDatabase.TabIndex = 1;
            this.radioButtonMySQLCreateDatabase.Text = "MySQL est installé, mais je n\'ai pas créé de base de données pour Sycorax.";
            this.radioButtonMySQLCreateDatabase.UseVisualStyleBackColor = true;
            this.radioButtonMySQLCreateDatabase.CheckedChanged += new System.EventHandler(this.radioButtonMySQL_CheckedChanged);
            // 
            // radioButtonMySQLNotYetInstalled
            // 
            this.radioButtonMySQLNotYetInstalled.AutoSize = true;
            this.radioButtonMySQLNotYetInstalled.Checked = true;
            this.radioButtonMySQLNotYetInstalled.Location = new System.Drawing.Point(43, 136);
            this.radioButtonMySQLNotYetInstalled.Name = "radioButtonMySQLNotYetInstalled";
            this.radioButtonMySQLNotYetInstalled.Size = new System.Drawing.Size(237, 17);
            this.radioButtonMySQLNotYetInstalled.TabIndex = 0;
            this.radioButtonMySQLNotYetInstalled.TabStop = true;
            this.radioButtonMySQLNotYetInstalled.Text = "À vrai dire, je n\'ai pas encore installé MySQL.";
            this.radioButtonMySQLNotYetInstalled.UseVisualStyleBackColor = true;
            this.radioButtonMySQLNotYetInstalled.CheckedChanged += new System.EventHandler(this.radioButtonMySQL_CheckedChanged);
            // 
            // wizardPageBase2
            // 
            this.wizardPageBase2.Controls.Add(this.labelMySQLServiceCheckResult);
            this.wizardPageBase2.Controls.Add(this.buttonMySQLServiceCheck);
            this.wizardPageBase2.Controls.Add(this.textBoxMySQLServiceName);
            this.wizardPageBase2.Controls.Add(this.label13);
            this.wizardPageBase2.Controls.Add(this.label12);
            this.wizardPageBase2.Controls.Add(this.label11);
            this.wizardPageBase2.Controls.Add(this.label10);
            this.wizardPageBase2.Controls.Add(this.label9);
            this.wizardPageBase2.Controls.Add(this.label8);
            this.wizardPageBase2.Controls.Add(this.label7);
            this.wizardPageBase2.Controls.Add(this.linkLabelMySQLDownload);
            this.wizardPageBase2.Controls.Add(this.label6);
            this.wizardPageBase2.Controls.Add(this.label5);
            this.wizardPageBase2.Description = "Mode d\'emploi pour une installation rapide du serveur MySQL.";
            this.wizardPageBase2.Index = 2;
            this.wizardPageBase2.Name = "wizardPageBase2";
            this.wizardPageBase2.Size = new System.Drawing.Size(625, 332);
            this.wizardPageBase2.TabIndex = 0;
            this.wizardPageBase2.Title = "Installation de MySQL";
            this.wizardPageBase2.WizardPageParent = this.wizardForm1;
            // 
            // labelMySQLServiceCheckResult
            // 
            this.labelMySQLServiceCheckResult.AutoSize = true;
            this.labelMySQLServiceCheckResult.Location = new System.Drawing.Point(44, 289);
            this.labelMySQLServiceCheckResult.Name = "labelMySQLServiceCheckResult";
            this.labelMySQLServiceCheckResult.Size = new System.Drawing.Size(0, 13);
            this.labelMySQLServiceCheckResult.TabIndex = 12;
            // 
            // buttonMySQLServiceCheck
            // 
            this.buttonMySQLServiceCheck.Enabled = false;
            this.buttonMySQLServiceCheck.Location = new System.Drawing.Point(180, 260);
            this.buttonMySQLServiceCheck.Name = "buttonMySQLServiceCheck";
            this.buttonMySQLServiceCheck.Size = new System.Drawing.Size(75, 23);
            this.buttonMySQLServiceCheck.TabIndex = 11;
            this.buttonMySQLServiceCheck.Text = "Vérification";
            this.buttonMySQLServiceCheck.UseVisualStyleBackColor = true;
            this.buttonMySQLServiceCheck.Click += new System.EventHandler(this.buttonMySQLServiceCheck_Click);
            // 
            // textBoxMySQLServiceName
            // 
            this.textBoxMySQLServiceName.Location = new System.Drawing.Point(55, 262);
            this.textBoxMySQLServiceName.Name = "textBoxMySQLServiceName";
            this.textBoxMySQLServiceName.Size = new System.Drawing.Size(119, 20);
            this.textBoxMySQLServiceName.TabIndex = 10;
            this.textBoxMySQLServiceName.TextChanged += new System.EventHandler(this.textBoxMySQLServiceName_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(41, 244);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(87, 13);
            this.label13.TabIndex = 9;
            this.label13.Text = "Nom du service :";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(41, 226);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(286, 13);
            this.label12.TabIndex = 8;
            this.label12.Text = "Vérifions si le service MySQL est bien installé et a démarré :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Firebrick;
            this.label11.Location = new System.Drawing.Point(41, 163);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(476, 26);
            this.label11.TabIndex = 7;
            this.label11.Text = "Faites toutefois attention à spécifier un mot de passe root sécurisé et notez le " +
                "nom\r\ndu service (par exemple MySQL5).";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(25, 203);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Étape 3";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(41, 117);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(525, 39);
            this.label9.TabIndex = 5;
            this.label9.Text = resources.GetString("label9.Text");
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(25, 98);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Étape 2";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(41, 63);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(384, 26);
            this.label7.TabIndex = 3;
            this.label7.Text = "Choisissez MySQL Community Server, la version open source sous licence GPL,\r\nensu" +
                "ite sélectionnez Windows (x86) ZIP/Setup.EXE";
            // 
            // linkLabelMySQLDownload
            // 
            this.linkLabelMySQLDownload.AutoSize = true;
            this.linkLabelMySQLDownload.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelMySQLDownload.Location = new System.Drawing.Point(41, 40);
            this.linkLabelMySQLDownload.Name = "linkLabelMySQLDownload";
            this.linkLabelMySQLDownload.Size = new System.Drawing.Size(237, 13);
            this.linkLabelMySQLDownload.TabIndex = 2;
            this.linkLabelMySQLDownload.TabStop = true;
            this.linkLabelMySQLDownload.Text = "http://dev.mysql.com/downloads/mysql/5.0.html";
            this.linkLabelMySQLDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(526, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Téléchargez MySQL 5.x (5.0 recommandé, mais si vous êtes aventureux, n\'hésitez pa" +
                "s à choisir la version 5.2)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(25, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Étape 1";
            // 
            // wizardFinalPage1
            // 
            this.wizardFinalPage1.BackColor = System.Drawing.Color.White;
            this.wizardFinalPage1.Description = "Sycorax est désormais prêt à fonctionner et à maintenir à jour votre base de donn" +
                "ées.           Pour modifier ultérieurement la configuration, lancez le Sycorax " +
                "Control Center.";
            this.wizardFinalPage1.Description2 = resources.GetString("wizardFinalPage1.Description2");
            this.wizardFinalPage1.FinishPage = true;
            this.wizardFinalPage1.HeaderImage = global::Sycorax.SetupWizard.Properties.Resources.PuzzleEnd;
            this.wizardFinalPage1.Index = 3;
            this.wizardFinalPage1.Name = "wizardFinalPage1";
            this.wizardFinalPage1.Size = new System.Drawing.Size(641, 396);
            this.wizardFinalPage1.TabIndex = 0;
            this.wizardFinalPage1.Title = "Félicitations";
            this.wizardFinalPage1.WelcomePage = true;
            this.wizardFinalPage1.WizardPageParent = this.wizardForm1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::Sycorax.SetupWizard.Properties.Resources.FrenchSetupWizard;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(289, 443);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // wizardPageBase3
            // 
            this.wizardPageBase3.Controls.Add(this.pictureBox2);
            this.wizardPageBase3.Controls.Add(this.buttonMySQLConnect1);
            this.wizardPageBase3.Controls.Add(this.textBoxPwd);
            this.wizardPageBase3.Controls.Add(this.label17);
            this.wizardPageBase3.Controls.Add(this.textBoxUid);
            this.wizardPageBase3.Controls.Add(this.label16);
            this.wizardPageBase3.Controls.Add(this.label15);
            this.wizardPageBase3.Controls.Add(this.textBoxServer);
            this.wizardPageBase3.Controls.Add(this.label14);
            this.wizardPageBase3.Description = "Création de la base de données";
            this.wizardPageBase3.Index = 4;
            this.wizardPageBase3.Name = "wizardPageBase3";
            this.wizardPageBase3.Size = new System.Drawing.Size(625, 332);
            this.wizardPageBase3.TabIndex = 0;
            this.wizardPageBase3.Title = "Connexion MySQL";
            this.wizardPageBase3.WizardPageParent = this.wizardForm1;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(23, 12);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(186, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Étape 1 - Connexion au serveur";
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(56, 49);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(128, 20);
            this.textBoxServer.TabIndex = 1;
            this.textBoxServer.Text = "localhost";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(40, 28);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(50, 13);
            this.label15.TabIndex = 2;
            this.label15.Text = "Serveur :";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(43, 79);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(39, 13);
            this.label16.TabIndex = 3;
            this.label16.Text = "Login :";
            // 
            // textBoxUid
            // 
            this.textBoxUid.Location = new System.Drawing.Point(56, 98);
            this.textBoxUid.Name = "textBoxUid";
            this.textBoxUid.Size = new System.Drawing.Size(128, 20);
            this.textBoxUid.TabIndex = 4;
            this.textBoxUid.Text = "root";
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Location = new System.Drawing.Point(56, 148);
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.Size = new System.Drawing.Size(128, 20);
            this.textBoxPwd.TabIndex = 6;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(43, 128);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(59, 13);
            this.label17.TabIndex = 5;
            this.label17.Text = "Password :";
            // 
            // buttonMySQLConnect1
            // 
            this.buttonMySQLConnect1.Location = new System.Drawing.Point(43, 181);
            this.buttonMySQLConnect1.Name = "buttonMySQLConnect1";
            this.buttonMySQLConnect1.Size = new System.Drawing.Size(75, 23);
            this.buttonMySQLConnect1.TabIndex = 7;
            this.buttonMySQLConnect1.Text = "Connexion";
            this.buttonMySQLConnect1.UseVisualStyleBackColor = true;
            this.buttonMySQLConnect1.Click += new System.EventHandler(this.buttonMySQLConnect1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(120, 181);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(64, 23);
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            // 
            // frmWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 449);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmWizard";
            this.Text = "Sycorax Setup Wizard";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.wizardPageBase1.ResumeLayout(false);
            this.wizardPageBase1.PerformLayout();
            this.wizardPageBase2.ResumeLayout(false);
            this.wizardPageBase2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.wizardPageBase3.ResumeLayout(false);
            this.wizardPageBase3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private UtilityLibrary.Wizards.WizardForm wizardForm1;
        private UtilityLibrary.Wizards.WizardWelcomePage wizardWelcomePage1;
        private UtilityLibrary.Wizards.WizardFinalPage wizardFinalPage1;
        private UtilityLibrary.Wizards.WizardPageBase wizardPageBase1;
        private System.Windows.Forms.RadioButton radioButtonMySQLNotYetInstalled;
        private System.Windows.Forms.RadioButton radioButtonMySQLCreateTable;
        private System.Windows.Forms.RadioButton radioButtonMySQLCreateDatabase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButtonMySQLJustFillParameters;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private UtilityLibrary.Wizards.WizardPageBase wizardPageBase2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel linkLabelMySQLDownload;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button buttonMySQLServiceCheck;
        private System.Windows.Forms.TextBox textBoxMySQLServiceName;
        private System.Windows.Forms.Label labelMySQLServiceCheckResult;
        private UtilityLibrary.Wizards.WizardPageBase wizardPageBase3;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxPwd;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBoxUid;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button buttonMySQLConnect1;
        private System.Windows.Forms.PictureBox pictureBox2;




    }
}