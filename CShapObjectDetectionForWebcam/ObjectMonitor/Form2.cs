using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectMonitor
{
    public partial class Form2 : Form
    {
        Form1 frm;
        bool isDB;
        public Form2(Form1 f, bool isDB)
        {
            frm = f;
            this.isDB = isDB;
            if (isDB)
                this.Text = "Database setting";
            else
                this.Text = "IPCamera setting";

            InitializeComponent();
            if (isDB)
                textBox1.Text = "localhost";
            else
                textBox1.Text = "http://192.168.2.133/mjpg/video.mjpg";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!isDB)
            {
                frm.ipcam_addr = textBox1.Text;
                frm.ipcam_username = textBox2.Text;
                frm.ipcam_password = textBox3.Text;
            }
            else
            {
                frm.db_addr = textBox1.Text;
                frm.db_username = textBox2.Text;
                frm.db_password = textBox3.Text;
            }
            Close();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!isDB)
            {
                frm.ipcam_addr = "";
                frm.ipcam_username = "";
                frm.ipcam_password = "";
            }
            else
            {
                frm.db_addr = textBox1.Text;
                frm.db_username = textBox2.Text;
                frm.db_password = textBox3.Text;
            }

            Close();
        }
    }
}
