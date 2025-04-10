using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls.WebParts;
using MySql.Data.MySqlClient;

namespace TESTME
{
    public partial class PopUpAjout : Form
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
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();



  
        public PopUpAjout()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 16, 16));

           
        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel2_Click(object sender, EventArgs e)
        {

        }

        private void bunifuHSlider1_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
        {
            bunifuLabel2.Text = "Nombre de joueurs : " + bunifuHSlider1.Value.ToString();
        }

        private void bunifuHSlider2_Scroll(object sender, Utilities.BunifuSlider.BunifuHScrollBar.ScrollEventArgs e)
        {
            bunifuLabel3.Text = "Nombre de cartes : " + bunifuHSlider2.Value.ToString();
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            string name = bunifuTextBox1.Text.Trim();
            string description = bunifuTextBox2.Text.Trim();
            int numPlayers = (int)bunifuHSlider1.Value;
            int numCards = (int)bunifuHSlider2.Value;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
               
                    string query = "INSERT INTO games (nom, description, nombre_joueurs, nombre_cartes) VALUES (@nom, @description, @nombre_joueurs, @nombre_cartes)";
                MySqlParameter[] parameters = {
                        new MySqlParameter("@nom", name),
                new MySqlParameter("@description", description),
                new MySqlParameter("@nombre_joueurs", numPlayers),
                new MySqlParameter("@nombre_cartes", numCards),
                };
                if (Database.ExecuteNonQuery(query, parameters))
                {
                    MessageBox.Show("Jeu ajouté avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Fermer proprement le popup sans fermer Admin
                    this.DialogResult = DialogResult.OK;
                }

                    

                  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout du jeu : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
