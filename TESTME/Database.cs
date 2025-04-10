using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;
using Bunifu.UI.WinForms;

namespace TESTME
{
    public class Database
    {
        private static string connectionString = "Server=localhost;Database=multi_jeux;User ID=root;Password=;";
        private static MySqlConnection conn = new MySqlConnection(connectionString);

        // Méthode pour récupérer la connexion unique
        public static MySqlConnection GetConnection()
        {
            if (conn.State == ConnectionState.Closed || conn.State == ConnectionState.Broken)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur de connexion à la base de données : {ex.Message}");
                }
            }
            return conn;
        }

        // Méthode pour exécuter une requête SELECT et retourner un DataTable
        public static DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, GetConnection()))
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'exécution de la requête : {ex.Message}");
            }
            return dataTable;
        }
        public static DataTable ExecuteQuery(string query, MySqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, GetConnection()))
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'exécution de la requête : {ex.Message}");
            }
            return dataTable;
        }

        // Méthode pour exécuter une requête INSERT, UPDATE, DELETE
        public static bool ExecuteNonQuery(string query, MySqlParameter[] parameters)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, GetConnection()))
                {
                    cmd.Parameters.AddRange(parameters);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'exécution de la requête : {ex.Message}");
                return false;
            }
        }
        public static bool ExecuteNonQuery(string query, MySqlParameter[] parameters, out long lastInsertedId)
        {
            lastInsertedId = -1; // Initialize to an error value

            try
            {
                using (MySqlConnection connection = GetConnection())
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddRange(parameters);
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0) // Check if the insert actually happened
                    {
                        lastInsertedId = cmd.LastInsertedId;
                        return true;
                    }
                    else
                    {
                        return false; // Insert failed
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'exécution de la requête : {ex.Message}");
                return false;
            }
        }
       
        public static DataTable LoadGames()
        {
            try
            {

                string query = "SELECT * FROM games";

                return Database.ExecuteQuery(query);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de chargement des jeux : {ex.Message}");
                return null;
            }

        }


        // 🔐 Fonction pour hasher un mot de passe avec SHA256
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Convertit chaque byte en hexadécimal
                }
                return builder.ToString();
            }
        }
        // Méthode pour vérifier si un mot de passe correspond au hash stocké
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashedInput = ComputeSha256Hash(inputPassword);
            return hashedInput == storedHash;
        }
    }
}
