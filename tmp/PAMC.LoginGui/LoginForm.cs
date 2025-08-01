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
                    // Create a SqlConnectionStringBuilder with the retrieved connection string
                    builder = new SqlConnectionStringBuilder(FileLogin.DatabaseConnections["Default"]);
                }
                else
                {
                    // Initialize builder to avoid NullReferenceException
                    builder = new SqlConnectionStringBuilder();
                }


                txtDatabase.Text = builder.InitialCatalog;
                txtServer.Text = builder.DataSource;
                txtUsername.Text = builder.UserID;

            }
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
                builder.UserID = txtUsername.Text;
                builder.Password = txtPassword.Text;

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
            //obscure error detail for enhanced security
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
    }
}
