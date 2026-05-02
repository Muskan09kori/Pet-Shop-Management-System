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
    public partial class CashForm : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        dbConnect dbCon = new dbConnect();
        SqlDataReader dr;
        String title = "Pet Shop Management System";
        MainForm main;

        public CashForm(MainForm form)
        {
            InitializeComponent();
            cn = new SqlConnection(dbCon.connection());
            main = form;
            getTransno();
            loadCash();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CashProduct product = new CashProduct(this);
            product.uname = main.lblUsername.Text;
            product.ShowDialog();
        }

        private void btnCash_Click(object sender, EventArgs e)
        {
            CashCustomer customer = new CashCustomer(this);
            customer.ShowDialog();

            if(MessageBox.Show("Are you sure you want to cash this product?", "cashing", MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
            {
                getTransno();
                main.loadDailySale();
                for(int i =0; i < dgvCash.Rows.Count; i++)
                {
                    dbCon.executeQuery("UPDATE tbProduct SET pqty = pqty -" + int.Parse(dgvCash.Rows[i].Cells[4].Value.ToString()) +"WHERE pcode LIKE "+ dgvCash.Rows[i].Cells[2].Value.ToString() + "");
                }
                dgvCash.Rows.Clear();
            }
        }

        private void dgvCash_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvCash.Columns[e.ColumnIndex].Name;
            string cashId = dgvCash.Rows[e.RowIndex].Cells[1].Value.ToString();

            if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this record?", "Delete Cash Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        cm = new SqlCommand("DELETE FROM tbCash WHERE cashid=@cashid", cn); // FIX: parameterized + space
                        cm.Parameters.AddWithValue("@cashid", cashId);
                        if (cn.State == ConnectionState.Closed) cn.Open();
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("Cash record has been successfully deleted!", title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        loadCash();
                    }
                    catch (Exception ex)
                    {
                        if (cn.State == ConnectionState.Open) cn.Close();
                        MessageBox.Show(ex.Message, title);
                    }
                }
            }
            else if (colName == "Increase")
            {
                try
                {
                    int i = checkPqty(dgvCash.Rows[e.RowIndex].Cells[2].Value.ToString());
                    int currentQtyInCart = int.Parse(dgvCash.Rows[e.RowIndex].Cells[4].Value.ToString());

                    if (currentQtyInCart < i)
                    {
                        cm = new SqlCommand("UPDATE tbCash SET qty = qty + 1 WHERE cashid=@cashid", cn); 
                        cm.Parameters.AddWithValue("@cashid", cashId);
                        if (cn.State == ConnectionState.Closed) cn.Open();
                        cm.ExecuteNonQuery();
                        cn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Remaining quantity on hand is " + i + "!", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    loadCash();
                }
                catch (Exception ex)
                {
                    if (cn.State == ConnectionState.Open) cn.Close();
                    MessageBox.Show(ex.Message, title);
                }
            }
            else if (colName == "Decrease")
            {
                try
                {
                    int qty = int.Parse(dgvCash.Rows[e.RowIndex].Cells[4].Value.ToString());
                    if (qty == 1)
                    {
                        // FIX: removed goto, just call delete directly
                        if (MessageBox.Show("Are you sure you want to delete this record?", "Delete Cash Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            cm = new SqlCommand("DELETE FROM tbCash WHERE cashid=@cashid", cn);
                            cm.Parameters.AddWithValue("@cashid", cashId);
                            if (cn.State == ConnectionState.Closed) cn.Open();
                            cm.ExecuteNonQuery();
                            cn.Close();
                            MessageBox.Show("Cash record has been successfully deleted!", title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        cm = new SqlCommand("UPDATE tbCash SET qty=qty-1 WHERE cashid=@cashid", cn); // FIX: parameterized + space
                        cm.Parameters.AddWithValue("@cashid", cashId);
                        if (cn.State == ConnectionState.Closed) cn.Open();
                        cm.ExecuteNonQuery();
                        cn.Close();
                    }
                    loadCash();
                }
                catch (Exception ex)
                {
                    if (cn.State == ConnectionState.Open) cn.Close();
                    MessageBox.Show(ex.Message, title);
                }
            }
        }

        #region method

        public void getTransno()
        {
            try
            {
                string sDate = DateTime.Now.ToString("yyyyMMdd");
                int count = 0;
                string transno;

                if (cn.State == ConnectionState.Closed) cn.Open();
                cm = new SqlCommand("SELECT COUNT(*) FROM tbCash WHERE transno LIKE @transno", cn);
                cm.Parameters.AddWithValue("@transno", sDate + "%");
                dr = cm.ExecuteReader();
                dr.Read();

                if (dr.HasRows)
                {
                    count = int.Parse(dr[0].ToString());
                    lblTransno.Text = sDate + (count + 1001).ToString();
                }
                else
                {
                    transno = sDate + "1001";
                    lblTransno.Text = transno;
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

        public void loadCash()
        {
            try
            {
                int i = 0;
                double total = 0;
                dgvCash.Rows.Clear();
                cm = new SqlCommand(@"SELECT cash.cashid, cash.pcode, cash.pname, cash.qty, 
                                     cash.price, cash.total, c.name, cash.cashier 
                                     FROM tbCash as cash 
                                     LEFT JOIN tbCustomer c ON cash.cid = c.id 
                                     WHERE cash.transno LIKE @transno", cn);
                cm.Parameters.AddWithValue("@transno", lblTransno.Text);
                if (cn.State == ConnectionState.Closed) cn.Open();
                dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    dgvCash.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(),
                                     dr[3].ToString(), dr[4].ToString(), dr[5].ToString(),
                                     dr[6].ToString(), dr[7].ToString());
                    total += double.Parse(dr[5].ToString());
                }
                dr.Close();
                cn.Close();
                lblTotal.Text = total.ToString("#,##0.00");
            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open) cn.Close();
                MessageBox.Show(ex.Message, title);
            }
        }

        public int checkPqty(string pcode)
        {
            int i = 0;
            try
            {
                cn.Open();
                cm = new SqlCommand("SELECT pqty FROM tbProduct WHERE pcode LIKE'" + pcode + "'", cn);
                i = int.Parse(cm.ExecuteScalar().ToString());
                cn.Close();
            }
            catch (Exception ex)
            {
                if (cn.State == ConnectionState.Open) cn.Close();
                MessageBox.Show(ex.Message, title);
            }
            return i;
        }
        #endregion method
    }
}