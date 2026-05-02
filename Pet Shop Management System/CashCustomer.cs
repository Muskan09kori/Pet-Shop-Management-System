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

namespace Pet_Shop_Management_System
{
    public partial class CashCustomer : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        dbConnect dbCon = new dbConnect();
        SqlDataReader dr;
        String title = "Pet Shop Management System";
        CashForm cash;

        public CashCustomer(CashForm form)
        {
            InitializeComponent();
            cn = new SqlConnection(dbCon.connection());
            cash = form;
            LoadCustomer();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadCustomer();
        }

        private void dgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // FIX: header click guard

            string colName = dgvCustomer.Columns[e.ColumnIndex].Name;
            if (colName == "Choice")
            {
                try
                {
                    // FIX: parameterized query, added space before WHERE, proper transno
                    cm = new SqlCommand("UPDATE tbCash SET cid=@cid WHERE transno=@transno", cn);
                    cm.Parameters.AddWithValue("@cid", dgvCustomer.Rows[e.RowIndex].Cells[1].Value.ToString());
                    cm.Parameters.AddWithValue("@transno", cash.lblTransno.Text);
                    if (cn.State == ConnectionState.Closed) cn.Open();
                    cm.ExecuteNonQuery();
                    cn.Close();
                    cash.loadCash();
                    this.Dispose();
                }
                catch (Exception ex)
                {
                    if (cn.State == ConnectionState.Open) cn.Close();
                    MessageBox.Show(ex.Message, title);
                }
            }
        }

        #region method

        public void LoadCustomer()
        {
            try
            {
                int i = 0;
                dgvCustomer.Rows.Clear();
                // FIX: correct CONCAT syntax with proper columns
                cm = new SqlCommand("SELECT id, name, phone FROM tbCustomer WHERE CONCAT(name, phone) LIKE @search", cn);
                cm.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                if (cn.State == ConnectionState.Closed) cn.Open();
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dgvCustomer.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
                }
                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open) cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }

        #endregion method
    }
}