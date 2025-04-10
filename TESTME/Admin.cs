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
using System.Xml.Linq;
using MySql.Data.MySqlClient;

namespace TESTME
{
    public partial class Admin : Form
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


     
    
        public Admin()
        {
            InitializeComponent();

            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 16, 16));

            bunifuDataGridView1.DataSource = Database.LoadGames();
        

            // Abonner à l'événement RowValidated pour le DataGridView
            bunifuDataGridView1.RowValidated += dataGridView1_RowValidated;

            bunifuTextBox1.TextChanged += bunifuTextBox1_TextChanged;

        }


        private void bunifuDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {
            string filterText = bunifuTextBox1.Text.Trim();
            (bunifuDataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"nom LIKE '%{filterText}%'";
        }

        private void dataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            // No need to check dataTable.GetChanges() anymore

            try
            {
                // Get the current row's data directly from the DataGridView
                DataGridViewRow row = bunifuDataGridView1.Rows[e.RowIndex];

                // Check if the row is a new row (if you have adding enabled)
                if (row.IsNewRow) return; // If the row is new, it will be handled on row add

                string query = "UPDATE games SET nom=@nom, description=@description, nombre_joueurs=@nombre_joueurs, nombre_cartes=@nombre_cartes WHERE id=@id";
                MySqlParameter[] parameters = {
            new MySqlParameter("@id", row.Cells["id"].Value),
            new MySqlParameter("@nom", row.Cells["nom"].Value),
            new MySqlParameter("@description", row.Cells["description"].Value),
            new MySqlParameter("@nombre_joueurs", row.Cells["nombre_joueurs"].Value),
            new MySqlParameter("@nombre_cartes", row.Cells["nombre_cartes"].Value),
        };

                Database.ExecuteNonQuery(query, parameters);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour du jeu : {ex.Message}");
                // Consider refreshing the row to revert changes if an error occurs
                bunifuDataGridView1.Refresh(); // Or refresh the specific row: bunifuDataGridView1.Rows[e.RowIndex].Refresh();
            }
        }

        private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            bunifuDataGridView1.EndEdit(); // Force validation

            DataGridViewRow gridRow = bunifuDataGridView1.Rows[e.RowIndex];

            // Check if it's a *new* row being added
            if (gridRow.IsNewRow) return;  // Do nothing if it's the placeholder row

            // IMPORTANT: Check if the row is actually *adding* a new record
            if (gridRow.DataBoundItem == null) // This is the key check
            {
                try
                {
                    string query = "INSERT INTO games (nom, description, nombre_joueurs, nombre_cartes) VALUES (@nom, @description, @nombre_joueurs, @nombre_cartes)";
                    MySqlParameter[] parameters = {
                new MySqlParameter("@nom", gridRow.Cells["nom"].Value),
                new MySqlParameter("@description", gridRow.Cells["description"].Value),
                new MySqlParameter("@nombre_joueurs", gridRow.Cells["nombre_joueurs"].Value),
                new MySqlParameter("@nombre_cartes", gridRow.Cells["nombre_cartes"].Value),
            };

                    long newId = -1;
                    if (Database.ExecuteNonQuery(query, parameters, out newId))
                    {
                        // Set the ID in the DataGridView *after* successful insert
                        gridRow.Cells["id"].Value = newId;  // Update the row's ID cell
                        MessageBox.Show("Jeu ajouté avec succès !");
                    }
                    else
                    {
                        e.Cancel = true; // Prevent row from validating if insert fails
                        MessageBox.Show("Erreur lors de l'ajout du jeu."); // More descriptive message
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'ajout du jeu : {ex.Message}");
                    e.Cancel = true; // Very important: prevent the row from being validated
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {

            PopUpAjout popup = new PopUpAjout();

            if (popup.ShowDialog() == DialogResult.OK)
            {
                bunifuDataGridView1.DataSource = Database.LoadGames() ; // Rafraîchir la liste des jeux après ajout
            }
        }

        private void boutonSupprimer_Click(object sender, EventArgs e)
        {
            if (bunifuDataGridView1.SelectedRows.Count > 0)
            {
                // Récupérer l'ID du jeu sélectionné
                int gameId = Convert.ToInt32(bunifuDataGridView1.SelectedRows[0].Cells["id"].Value);

                DialogResult confirm = MessageBox.Show("Voulez-vous vraiment supprimer ce jeu ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                     
                            string query = "DELETE FROM games WHERE id = @id";
                        MySqlParameter[] parameters = {
                new MySqlParameter("@id", gameId),
                        };
                        if (Database.ExecuteNonQuery(query, parameters))
                        {

                            MessageBox.Show("Jeu supprimé avec succès !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Rafraîchir la liste après suppression
                            bunifuDataGridView1.DataSource = Database.LoadGames();
                        }


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression du jeu : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un jeu à supprimer.", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void bunifuTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }


}
