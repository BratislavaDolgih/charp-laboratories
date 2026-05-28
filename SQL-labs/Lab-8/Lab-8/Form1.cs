using Npgsql;
using System;
using System.Windows.Forms;

namespace AgroDbApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            TestConnection();
        }

        private void TestConnection()
        {
            string connString = "Host=localhost;Port=5432;Username=postgres;Password=1234567;Database=agro_db";

            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                MessageBox.Show("Подключение к PostgreSQL успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения: " + ex.Message);
            }
        }
    }
}