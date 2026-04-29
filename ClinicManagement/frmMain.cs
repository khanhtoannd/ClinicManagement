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

        private void mnSpecialties_Click(object sender, EventArgs e)
        {
            frmSpecialtiesLINQ f = new frmSpecialtiesLINQ();
            f.ShowDialog();
        }

        private void mnAppointments_Click(object sender, EventArgs e)
        {
            frmAppointmentsLINQ f = new frmAppointmentsLINQ();
            f.ShowDialog();
        }

        private void mnMedicalRecords_Click(object sender, EventArgs e)
        {
            frmMedicalRecordsLINQ f = new frmMedicalRecordsLINQ();
            f.ShowDialog();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void seviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmServicesLINQ f = new frmServicesLINQ();
            f.ShowDialog();
        }

        private void medicalRecordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmMedicalRecordsLINQ f = new frmMedicalRecordsLINQ();
            f.ShowDialog();
        }
        private void invoicesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmInvoicesLINQ f = new frmInvoicesLINQ();
            f.ShowDialog();
        }
        private void invoiceDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmInvoiceDetailsLINQ f = new frmInvoiceDetailsLINQ();
            f.ShowDialog();
        }
        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStatisticsLINQ f = new frmStatisticsLINQ();
            f.ShowDialog();
        }
    }
}