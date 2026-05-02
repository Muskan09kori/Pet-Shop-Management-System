using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Pet_Shop_Management_System
{
    class dbConnect
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        private string con;

        public String connection()
        {
            con = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hp\source\repos\Pet Shop Management System\Pet Shop Management System\dbPetShop.mdf;Integrated Security=True;Connect Timeout=30";
            return con;
        }

        public void executeQuery(String query)
        {
            try
            {
                cn.ConnectionString = connection();
                cn.Open();
                cm = new SqlCommand(query, cn);
                cm.ExecuteNonQuery(); // FIX: this was missing, query was never actually executed!
                cn.Close();
            }
            catch (Exception ex) // FIX: added 'ex'
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}