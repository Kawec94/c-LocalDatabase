using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace LocalDatabaseApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DataBaseConnection DBConn = new DataBaseConnection();

        public bool isLoginSucess;

        void buttonLogin_Click(object sender, EventArgs e)
        {
            if (DBConn.loginUser(textBoxLogin.Text, textBoxPassword.Text))
            {
                isLoginSucess = true;
                this.Close();
            }
        }

        void buttonRegister_Click(object sender, EventArgs e)
        {
            DBConn.registerUser(textBoxLogin.Text, textBoxPassword.Text);
        }
    }
}
