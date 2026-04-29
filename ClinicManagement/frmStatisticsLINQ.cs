using System;
using System.Linq;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmStatisticsLINQ : Form
    {
        public frmStatisticsLINQ()
        {
            InitializeComponent();
        }

        private void setReadOnly()
        {
            txtTotalAppointments.ReadOnly = true;
            txtTotalMedicalRecords.ReadOnly = true;
            txtTotalInvoices.ReadOnly = true;
            txtTotalRevenue.ReadOnly = true;
            txtPaidInvoices.ReadOnly = true;
            txtUnpaidInvoices.ReadOnly = true;
        }

        private void LoadStatistics()
        {
            DateTime fromDate = dtpFromDate.Value.Date;
            DateTime toDate = dtpToDate.Value.Date.AddDays(1);

            using (DataContext db = new DataContext())
            {
                int totalAppointments = db.Appointments
                    .Count(a => a.AppointmentDate >= fromDate &&
                                a.AppointmentDate < toDate);

                int totalMedicalRecords = db.MedicalRecords
                    .Count(m => m.RecordDate >= fromDate &&
                                m.RecordDate < toDate);

                int totalInvoices = db.Invoices
                    .Count(i => i.InvoiceDate >= fromDate &&
                                i.InvoiceDate < toDate);

                decimal totalRevenue = db.Invoices
                    .Where(i => i.InvoiceDate >= fromDate &&
                                i.InvoiceDate < toDate)
                    .Select(i => (decimal?)i.PaidAmount)
                    .Sum() ?? 0;

                int paidInvoices = db.Invoices
                    .Count(i => i.InvoiceDate >= fromDate &&
                                i.InvoiceDate < toDate &&
                                i.Status == 1);

                int unpaidInvoices = db.Invoices
                    .Count(i => i.InvoiceDate >= fromDate &&
                                i.InvoiceDate < toDate &&
                                i.Status == 0);

                txtTotalAppointments.Text = totalAppointments.ToString();
                txtTotalMedicalRecords.Text = totalMedicalRecords.ToString();
                txtTotalInvoices.Text = totalInvoices.ToString();
                txtTotalRevenue.Text = totalRevenue.ToString("N0");
                txtPaidInvoices.Text = paidInvoices.ToString();
                txtUnpaidInvoices.Text = unpaidInvoices.ToString();
            }
        }

        private void frmStatisticsLINQ_Load(object sender, EventArgs e)
        {
            dtpFromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtpToDate.Value = DateTime.Now;

            setReadOnly();
            LoadStatistics();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (dtpFromDate.Value.Date > dtpToDate.Value.Date)
            {
                MessageBox.Show("From Date must be less than or equal to To Date.");
                return;
            }

            LoadStatistics();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }
    }
}