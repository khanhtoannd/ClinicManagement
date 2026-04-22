using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private string GetMD5(string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string sql = @"SELECT * FROM tblUsers
                           WHERE Username = @Username
                           AND Password = @Password
                           AND Status = 1";

            SqlParameter[] pr = new SqlParameter[]
            {
                new SqlParameter("@Username", txtUsername.Text.Trim()),
                new SqlParameter("@Password", GetMD5(txtPassword.Text.Trim()))
            };

            DBServices db = new DBServices();
            DataTable dt = db.GetData(sql, pr);

            if (dt.Rows.Count > 0)
            {
                frmMain f = new frmMain();
                this.Hide();
                f.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Wrong username or password.");
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }


        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}