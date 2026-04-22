using System;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
        }

        private void mnUsersLINQ_Click(object sender, EventArgs e)
        {
            frmUsersLINQ f = new frmUsersLINQ();
            f.ShowDialog();
        }

        private void mnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnUsers_Click(object sender, EventArgs e)
        {
            frmUsersLINQ f = new frmUsersLINQ();
            f.ShowDialog();
        }

        private void mnPatients_Click(object sender, EventArgs e)
        {
            frmPatientsLINQ f = new frmPatientsLINQ();
            f.ShowDialog();
        }


        private void mnDoctors_Click(object sender, EventArgs e)
        {

            frmDoctorsLINQ f = new frmDoctorsLINQ();
            f.ShowDialog();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }
    }
}