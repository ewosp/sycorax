using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sycorax;

namespace Sycorax.ControlCenter {
    /// <summary>
    /// Probe MySQL Connection Form
    /// </summary>
    public partial class ProbeMySqlConnectionForm : Form {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ProbeMySqlConnectionForm"/> class.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        public ProbeMySqlConnectionForm (string ConnectionString) {
            InitializeComponent();
            probeConnection = new ProbeMySqlConnection(ConnectionString);
            probeConnection.StatusChange += new EventHandler<MessageEventArgs>(probeConnection_StatusChange);
            probeConnection.Error += new EventHandler<ExceptionEventArgs>(probeConnection_Error);
        }

        /// <summary>
        /// Handles the Error event of the probeConnection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Sycorax.ExceptionEventArgs"/> instance containing the exception.</param>
        void probeConnection_Error (object sender, ExceptionEventArgs e) {
            textBoxException.Visible = true;
            textBoxException.Text = e.RaisedException.Message;
        }

        /// <summary>
        /// Handles the StatusChange event of the probeConnection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Sycorax.MessageEventArgs"/> instance containing the message.</param>
        void probeConnection_StatusChange (object sender, MessageEventArgs e) {
            textBoxStatus.Text = e.Message;
        }

        /// <summary>
        /// Handles the Load event of the ProbeConnectionMySQL control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        private void ProbeConnectionMySQL_Load (object sender, EventArgs e) {
            probeConnection.Open();
        }

        /// <summary>
        /// Our Probe MySQL Connection class
        /// </summary>
        private ProbeMySqlConnection probeConnection;

        /// <summary>
        /// Handles the FormClosed event of the ProbeMySqlConnectionForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:System.Windows.Forms.FormClosedEventArgs"/> instance containing the event data.</param>
        private void ProbeMySqlConnectionForm_FormClosed (object sender, FormClosedEventArgs e) {
            //Close MySQL connection and clean ressources
            probeConnection.Dispose();
        }
    }
}