using PAMC.DatabaseConnection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PAMC.LoginGui
{


    public partial class LoginForm : Form
    {
        public bool Continue { get; set; }
        SqlConnection _cn = null;
        SqlConnectionStringBuilder builder = null;

        public LoginForm()
        {
            InitializeComponent();
            FileLogin.DecryptFilePopulateConnections();
            if (FileLogin.DatabaseConnections != null && FileLogin.DatabaseConnections["Default"].Length > 0)
            {
                string defaultConnectionString = FileLogin.DatabaseConnections["Default"];
                if (!string.IsNullOrEmpty(defaultConnectionString))
                {
                    builder = new SqlConnectionStringBuilder(FileLogin.DatabaseConnections["Default"]);
                }
                else
                {
                    builder = new SqlConnectionStringBuilder();
                }
                txtDatabase.Text = builder.InitialCatalog;
                txtServer.Text = builder.DataSource;
                txtUsername.Text = builder.UserID;
                chkWindowsAuth.Checked = builder.IntegratedSecurity;
            }
            ToggleAuthControls();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (builder == null)
            {
                builder = new SqlConnectionStringBuilder();
            }
            try
            {
                builder.InitialCatalog = txtDatabase.Text;
                builder.DataSource = txtServer.Text;
                if (chkWindowsAuth.Checked)
                {
                    builder.IntegratedSecurity = true;
                    builder.UserID = string.Empty;
                    builder.Password = string.Empty;
                }
                else
                {
                    builder.UserID = txtUsername.Text;
                    builder.Password = txtPassword.Text;
                    builder.IntegratedSecurity = false;
                }
                builder.Encrypt = true;
                builder.TrustServerCertificate = true;
                _cn = new SqlConnection(builder.ConnectionString);
                _cn.Open();
                Continue = true;
                _cn.Close();
                this.Close();
                try
                {
                    FileLogin.UpdateLoginFile("Default", builder.ConnectionString);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (SqlException)
            {
                MessageBox.Show("Unable to connect to the database. Please check your connection details.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("An unexpected error occurred while attempting to connect. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("An unexpected error occurred. Please contact support if the problem persists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_cn != null)
                {
                    _cn.Dispose();
                }
            }
        }

        private void chkWindowsAuth_CheckedChanged(object sender, EventArgs e)
        {
            ToggleAuthControls();
        }

        private void ToggleAuthControls()
        {
            bool useWindowsAuth = chkWindowsAuth.Checked;
            txtUsername.Enabled = !useWindowsAuth;
            txtPassword.Enabled = !useWindowsAuth;
        }
    }
}
