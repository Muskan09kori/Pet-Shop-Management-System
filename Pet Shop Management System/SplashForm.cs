using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pet_Shop_Management_System
{
    public partial class SplashForm : Form
    {
        private int startPoint = 0;

        public SplashForm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            startPoint += 2;
            guna2ProgressBar1.Value = startPoint;

            // Debug: Check the value in Output window
            System.Diagnostics.Debug.WriteLine("Progress: " + startPoint);

            if (guna2ProgressBar1.Value >= 100)
            {
                guna2ProgressBar1.Value = 100;
                timer1.Stop();
                LoginForm login = new LoginForm();
                login.ShowDialog();
                this.Hide();
             
            }
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            // Start timer here
            guna2ProgressBar1.Minimum = 0;
            guna2ProgressBar1.Maximum = 100;
            guna2ProgressBar1.Value = 0;
            startPoint = 0;

            // Make sure progress bar is visible
            guna2ProgressBar1.Visible = true;
            guna2ProgressBar1.BringToFront();

            // Configure and start timer
            timer1.Interval = 50;
            timer1.Start();
        }

        // Remove the label click methods or keep them empty if needed
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void timer2_Tick(object sender, EventArgs e) { }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}