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
    public partial class CashProduct : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        dbConnect dbCon = new dbConnect();
        SqlDataReader dr;
        String title = "Pet Shop Management System";
        public string uname;
        CashForm cash;

        public CashProduct(CashForm form)
        {
            InitializeComponent();
            cn = new SqlConnection(dbCon.connection());
            cash = form;
            LoadProduct();
        }

        private void btnCash_Click(object sender, EventArgs e)
        {
            bool anySelected = false;

            foreach (DataGridViewRow row in dgvProduct.Rows)
            {
                if (row.Cells["Select"].Value == null) continue;

                bool chkBox = Convert.ToBoolean(row.Cells["Select"].Value);
                if (chkBox)
                {
                    anySelected = true;
                    try
                    {
                        if (cn.State == ConnectionState.Closed) cn.Open();

                        // 1. Check the actual stock available right now
                        SqlCommand checkStock = new SqlCommand("SELECT pqty FROM tbProduct WHERE pcode = @pcode", cn);
                        checkStock.Parameters.AddWithValue("@pcode", row.Cells[1].Value.ToString());
                        int currentStock = Convert.ToInt32(checkStock.ExecuteScalar());

                        // 2. Prevent checkout if it is out of stock (0 or less)
                        if (currentStock <= 0)
                        {
                            MessageBox.Show("Cannot cash out '" + row.Cells[2].Value.ToString() + "' because it is out of stock!", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            continue;
                        }

                        // 3. Process the sale (Insert to Cash table)
                        // Your SQL database Trigger will automatically subtract the inventory when this runs!
                        cm = new SqlCommand("INSERT INTO tbCash(transno,pcode,pname,qty,price,cashier) VALUES(@transno,@pcode,@pname,@qty,@price,@cashier)", cn);
                        cm.Parameters.AddWithValue("@transno", cash.lblTransno.Text);
                        cm.Parameters.AddWithValue("@pcode", row.Cells[1].Value.ToString());
                        cm.Parameters.AddWithValue("@pname", row.Cells[2].Value.ToString());
                        cm.Parameters.AddWithValue("@qty", 1);
                        cm.Parameters.AddWithValue("@price", Convert.ToDouble(row.Cells[5].Value.ToString()));
                        cm.Parameters.AddWithValue("@cashier", uname);
                        cm.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, title);
                    }
                    finally
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                    }
                }
            }

            if (!anySelected)
            {
                MessageBox.Show("Please select at least one product!", "Warning");
                return;
            }

            cash.loadCash();
            this.Dispose();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        #region Method

        public void LoadProduct()
        {
            try
            {
                int i = 0;
                dgvProduct.Rows.Clear();
                // AND pqty > 0 means only show products that are IN STOCK
                cm = new SqlCommand("SELECT pcode,pname,ptype,pcategory,pprice FROM tbProduct WHERE CONCAT(pname,ptype,pcategory) LIKE @search AND pqty > 0", cn);
                cm.Parameters.AddWithValue("@search", "%" + txtSearch.Text + "%");
                if (cn.State == ConnectionState.Closed) cn.Open();
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dgvProduct.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(),
                                        dr[3].ToString(), dr[4].ToString());
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

            #endregion Method
        }
}