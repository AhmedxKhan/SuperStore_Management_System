using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;

namespace SuperStore
{
    public partial class SignUp : Form
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void SignUp_Load(object sender, EventArgs e)
        {
            // Optional: set default focus
            txtNewUsername.Focus();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // ✅ Basic validation
            if (string.IsNullOrWhiteSpace(txtNewUsername.Text) ||
                string.IsNullOrWhiteSpace(txtNewPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text) ||
                cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Please fill all fields and select a role.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // ✅ Get connection string from App.config
                string connString = ConfigurationManager.ConnectionStrings["SuperStoreDB"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Insert user into Users table with role
                    string query = "INSERT INTO Users (Name, Password, Role) VALUES (@Name, @Password, @Role)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Name", txtNewUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", txtNewPassword.Text.Trim()); // ⚠ Later: hash password
                    cmd.Parameters.AddWithValue("@Role", cmbRole.SelectedItem.ToString()); // ✅ Only one role

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Registration successful! Please log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Go back to SignIn form
                        SignIn signinForm = new SignIn();
                        signinForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Registration failed. Try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkSignin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // ✅ Go back to SignIn form
            SignIn signinForm = new SignIn();
            signinForm.Show();
            this.Hide();
        }

        private void cmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
