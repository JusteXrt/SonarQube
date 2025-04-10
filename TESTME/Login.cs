using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace TESTME
{
    public partial class Login : Form
    {
       
            [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
            private static extern IntPtr CreateRoundRectRgn
            (
                int nLeftRect,     // x-coordinate of upper-left corner
                int nTopRect,      // y-coordinate of upper-left corner
                int nRightRect,    // x-coordinate of lower-right corner
                int nBottomRect,   // y-coordinate of lower-right corner
                int nWidthEllipse, // height of ellipse
                int nHeightEllipse // width of ellipse
            );


        
        public Login()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 16, 16));
            Database.GetConnection();

            /*
            using (conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // MessageBox.Show("Connexion réussie à la base de données MySQL.");
                    // Console.WriteLine("Connexion réussie à la base de données MySQL.");
                    // Ton code pour interagir avec la base de données
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur de connexion : {ex.Message}");
                    // Console.WriteLine($"Erreur de connexion : {ex.Message}");
                }
            }*/
        }

        private void bunifuPictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void bunifuLabel1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

      
        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            string str_username1 = username.Text;
            string str_password1 = password.Text;

            if (AuthenticateUser(str_username1, str_password1, out string role))
            {
                MessageBox.Show("Connexion réussie !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (role == "admin")
                {
                    Admin adminForm = new Admin();
                    adminForm.Show();
                }
                else
                {
                    Main userForm = new Main();
                    userForm.Show();
                }

                this.Hide(); // Cacher le formulaire de connexion
            }
            else
            {
                MessageBox.Show("Nom d'utilisateur ou mot de passe incorrect.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AuthenticateUser(string username, string password, out string role)
        {
            role = null;
            bool isAuthenticated = false;

            try
            {
                string query = "SELECT password, role FROM users WHERE username=@username";
                MySqlParameter[] parameters = {
            new MySqlParameter("@username", username)
        };

                DataTable userTable = Database.ExecuteQuery(query, parameters);

                if (userTable.Rows.Count > 0)
                {
                    string storedHashedPassword = userTable.Rows[0]["password"].ToString();
                    role = userTable.Rows[0]["role"].ToString();

                    // Vérifier si le mot de passe saisi correspond au hash stocké
                    if (Database.VerifyPassword(password, storedHashedPassword))
                    {
                        isAuthenticated = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de connexion : {ex.Message}");
            }

            return isAuthenticated;
        }

        private void password_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
