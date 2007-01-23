using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ServiceProcess;
using MySql.Data.MySqlClient;

namespace Sycorax.SetupWizard {
    public partial class frmWizard : Form {
        public frmWizard () {
            InitializeComponent();
            /*
            imageList.Images.Add(Properties.Resources.Start);
            wizardWelcomePage1.ImageList = imageList;
            wizardWelcomePage1.ImageIndex = 0;
             */

            //Radio buttons
            listRadioButtonMySQL = new List<RadioButton>();
            listRadioButtonMySQL.Add(radioButtonMySQLNotYetInstalled);
            listRadioButtonMySQL.Add(radioButtonMySQLCreateDatabase);
            listRadioButtonMySQL.Add(radioButtonMySQLCreateTable);
            listRadioButtonMySQL.Add(radioButtonMySQLJustFillParameters);

            //Links            
            InitializeLink(linkLabelMySQLDownload);
        }

        public void InitializeLink (LinkLabel linkLabel) {
            linkLabel.Links.Add(linkLabel.LinkArea.Start, linkLabel.LinkArea.Length, linkLabel.Text.ToString());
        }

        ImageList imageList = new ImageList();

        private void radioButtonMySQL_CheckedChanged (object sender, EventArgs e) {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked == false) return;
            foreach (RadioButton radioButtonMySQL in listRadioButtonMySQL) {
                if (radioButton != radioButtonMySQL) radioButtonMySQL.Checked = false;
            }
        }

        List<RadioButton> listRadioButtonMySQL;

        private void buttonMySQLServiceCheck_Click (object sender, EventArgs e) {
            try {
                ServiceController sc = new ServiceController(textBoxMySQLServiceName.Text);
                if (sc.Status != ServiceControllerStatus.Running) {
                    sc.Start();
                }
                labelMySQLServiceCheckResult.ForeColor = Color.DarkGreen;
                labelMySQLServiceCheckResult.Text = "Félicitations, MySQL est correctement installé :-)";   
            } catch (Exception ex) {
                labelMySQLServiceCheckResult.ForeColor = Color.Firebrick;
                labelMySQLServiceCheckResult.Text = ex.Message;
            }
        }

        private void textBoxMySQLServiceName_TextChanged (object sender, EventArgs e) {
            TextBox textBox = (TextBox)sender;
            buttonMySQLServiceCheck.Enabled = (textBox.Text.Length > 0);
        }

        private void linkLabel_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void buttonMySQLConnect1_Click (object sender, EventArgs e) {
            connRoot.ConnectionString = String.Format(
                "Server={0};Uid={1};Pwd={2}",
                textBoxServer.Text,
                textBoxUid.Text,
                textBoxPwd.Text
            );
            try {
                connRoot.Open();

            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Échec de la connexion MySQL");
            }
        }

        MySqlConnection connRoot;
        MySqlConnection connSycorax;


    }
}