using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LocalDatabaseApp
{
    public partial class mainForm : Form
    {

        Form1 loginForm = new Form1();
        DataBaseConnection DBConn = new DataBaseConnection();

        public mainForm()
        {
            InitializeComponent();
            this.Hide();
            loginForm.ShowDialog();
            this.Show();
            afterLogin();
            comboBox1.DataSource = DBConn.pupilsList();
        }

        private void buttonLoginRegister_Click(object sender, EventArgs e)
        {
            this.Hide();
            loginForm.ShowDialog();
            this.Show();
            afterLogin();
        }

        private void afterLogin()
        {
            if (loginForm.isLoginSucess)
            {
                buttonLoginRegister.Visible = false;
                dataGridView.Visible = true;
                comboBox1.Visible = true;
                buttonInsert.Visible = true;
                textBoxGrade.Visible = true;
                textBoxWeight.Visible = true;
                labelGrade.Visible = true;
                labelWeight.Visible = true;
                labelAverage.Visible = true;
            }
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            DBConn.insertData(comboBox1.SelectedIndex + 1, Int32.Parse(textBoxGrade.Text), Int32.Parse(textBoxWeight.Text));
            dataGridView.DataSource = DBConn.showDataSelected(comboBox1.SelectedIndex + 1);
            labelAverage.Text = "Średnia: " + DBConn.average(comboBox1.SelectedIndex + 1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView.DataSource = DBConn.showDataSelected(comboBox1.SelectedIndex + 1);
            labelAverage.Text = "Średnia: " + DBConn.average(comboBox1.SelectedIndex + 1);
        }
    }

}
