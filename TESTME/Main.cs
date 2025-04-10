using MySql.Data.MySqlClient;
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

namespace TESTME
{

    public partial class Main : Form
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


        private MySqlDataAdapter adapter;
        private DataTable dataTable;

        public Main()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 16, 16));
            bunifuDataGridView1.DataSource = Database.LoadGames();

            bunifuTextBox1.TextChanged += bunifuTextBox1_TextChanged;
        }
     
       
        private void bunifuTextBox1_TextChanged(object sender, EventArgs e)
        {
            string filterText = bunifuTextBox1.Text.Trim();
            (bunifuDataGridView1.DataSource as DataTable).DefaultView.RowFilter = $"nom LIKE '%{filterText}%'";
        }
        private void bunifuDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
