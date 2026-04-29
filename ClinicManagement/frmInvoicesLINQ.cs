using System;
using System.Linq;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmInvoicesLINQ : Form
    {
        private bool AddNew = false;

        public frmInvoicesLINQ()
        {
            InitializeComponent();
        }

        private void setControl(bool check)
        {
            txtInvoiceID.Enabled = false;

            cbbAppointmentID.Enabled = check;
            txtPatientID.Enabled = false;

            dtpInvoiceDate.Enabled = check;
            txtTotalAmount.Enabled = check;
            txtPaidAmount.Enabled = check;
            cbbStatus.Enabled = check;
            txtNote.Enabled = check;

            btnSave.Enabled = check;
            btnCancel.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            btnExit.Enabled = !check;

            dgvInvoices.Enabled = !check;
        }

        private void ClearInput()
        {
            txtInvoiceID.Clear();
            txtPatientID.Clear();
            txtTotalAmount.Text = "0";
            txtPaidAmount.Text = "0";
            txtNote.Clear();

            dtpInvoiceDate.Value = DateTime.Now;

            if (cbbAppointmentID.Items.Count > 0)
                cbbAppointmentID.SelectedIndex = 0;

            if (cbbStatus.Items.Count > 0)
                cbbStatus.SelectedIndex = 0;
        }

        private bool TryGetAppointmentID(out int appointmentID)
        {
            appointmentID = 0;

            if (cbbAppointmentID.SelectedValue == null)
                return false;

            return int.TryParse(cbbAppointmentID.SelectedValue.ToString(), out appointmentID);
        }

        private void LoadAppointments()
        {
            using (DataContext db = new DataContext())
            {
                var data = db.Appointments
                    .Select(a => new
                    {
                        a.AppointmentID,
                        DisplayText = "Appointment " + a.AppointmentID
                    })
                    .ToList();

                cbbAppointmentID.DataSource = data;
                cbbAppointmentID.DisplayMember = "DisplayText";
                cbbAppointmentID.ValueMember = "AppointmentID";
            }
        }

        private void LoadStatus()
        {
            cbbStatus.Items.Clear();
            cbbStatus.Items.Add("Unpaid");
            cbbStatus.Items.Add("Paid");
            cbbStatus.SelectedIndex = 0;
        }

        private int GetStatusValue()
        {
            if (cbbStatus.Text == "Paid")
                return 1;

            return 0;
        }

        private void LoadPatientByAppointment()
        {
            int appointmentID;

            if (!TryGetAppointmentID(out appointmentID))
                return;

            using (DataContext db = new DataContext())
            {
                var appointment = db.Appointments
                    .FirstOrDefault(a => a.AppointmentID == appointmentID);

                if (appointment != null)
                {
                    txtPatientID.Text = appointment.PatientID.ToString();
                }
                else
                {
                    txtPatientID.Clear();
                }
            }
        }

        private void LoadGridData()
        {
            using (DataContext db = new DataContext())
            {
                var data = from inv in db.Invoices
                           join ap in db.Appointments
                                on inv.AppointmentID equals ap.AppointmentID
                           join p in db.Patients
                                on ap.PatientID equals p.PatientID
                           select new
                           {
                               inv.InvoiceID,
                               inv.AppointmentID,
                               ap.PatientID,
                               PatientName = p.FullName,
                               inv.InvoiceDate,
                               inv.TotalAmount,
                               inv.PaidAmount,
                               StatusText = inv.Status == 1 ? "Paid" : "Unpaid",
                               inv.Note
                           };

                dgvInvoices.AutoGenerateColumns = false;
                dgvInvoices.DataSource = data.ToList();
            }

            setControl(false);
        }

        private void frmInvoicesLINQ_Load(object sender, EventArgs e)
        {
            dgvInvoices.AutoGenerateColumns = false;
            dgvInvoices.AllowUserToAddRows = false;
            dgvInvoices.ReadOnly = true;
            dgvInvoices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInvoices.MultiSelect = false;

            LoadStatus();
            LoadAppointments();
            LoadGridData();
            LoadPatientByAppointment();
        }

        private void cbbAppointmentID_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPatientByAppointment();
        }

        private void dgvInvoices_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvInvoices.Rows[e.RowIndex].Cells["InvoiceID"].Value == null)
                return;

            txtInvoiceID.Text =
                dgvInvoices.Rows[e.RowIndex].Cells["InvoiceID"].Value.ToString();

            cbbAppointmentID.SelectedValue =
                dgvInvoices.Rows[e.RowIndex].Cells["AppointmentID"].Value;

            txtPatientID.Text =
                dgvInvoices.Rows[e.RowIndex].Cells["PatientID"].Value.ToString();

            if (dgvInvoices.Rows[e.RowIndex].Cells["InvoiceDate"].Value != null)
            {
                dtpInvoiceDate.Value = Convert.ToDateTime(
                    dgvInvoices.Rows[e.RowIndex].Cells["InvoiceDate"].Value
                );
            }

            txtTotalAmount.Text =
                dgvInvoices.Rows[e.RowIndex].Cells["TotalAmount"].Value.ToString();

            txtPaidAmount.Text =
                dgvInvoices.Rows[e.RowIndex].Cells["PaidAmount"].Value.ToString();

            cbbStatus.Text =
                dgvInvoices.Rows[e.RowIndex].Cells["StatusText"].Value.ToString();

            txtNote.Text =
                dgvInvoices.Rows[e.RowIndex].Cells["Note"].Value?.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);
            ClearInput();

            cbbAppointmentID.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (txtInvoiceID.Text.Trim() == "")
            {
                MessageBox.Show("Please select an invoice to edit.");
                return;
            }

            AddNew = false;
            setControl(true);

            cbbAppointmentID.Enabled = false;
            txtTotalAmount.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setControl(false);

            if (dgvInvoices.CurrentRow != null)
            {
                int rowIndex = dgvInvoices.CurrentRow.Index;
                dgvInvoices_CellEnter(this, new DataGridViewCellEventArgs(0, rowIndex));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int appointmentID;

            if (!TryGetAppointmentID(out appointmentID))
            {
                MessageBox.Show("Please select an appointment.");
                return;
            }

            decimal totalAmount;
            decimal paidAmount;

            if (!decimal.TryParse(txtTotalAmount.Text.Trim(), out totalAmount))
            {
                MessageBox.Show("Total amount is invalid.");
                txtTotalAmount.Focus();
                return;
            }

            if (!decimal.TryParse(txtPaidAmount.Text.Trim(), out paidAmount))
            {
                MessageBox.Show("Paid amount is invalid.");
                txtPaidAmount.Focus();
                return;
            }

            using (DataContext db = new DataContext())
            {
                if (AddNew)
                {
                    bool exists = db.Invoices
                        .Any(x => x.AppointmentID == appointmentID);

                    if (exists)
                    {
                        MessageBox.Show("This appointment already has an invoice.");
                        return;
                    }

                    tblInvoices invoice = new tblInvoices();

                    invoice.AppointmentID = appointmentID;
                    invoice.InvoiceDate = dtpInvoiceDate.Value;
                    invoice.TotalAmount = totalAmount;
                    invoice.PaidAmount = paidAmount;
                    invoice.Status = GetStatusValue();
                    invoice.Note = txtNote.Text.Trim();

                    db.Invoices.Add(invoice);
                    db.SaveChanges();

                    MessageBox.Show("Invoice added successfully.");
                }
                else
                {
                    int invoiceID = Convert.ToInt32(txtInvoiceID.Text);

                    tblInvoices invoice = db.Invoices
                        .FirstOrDefault(x => x.InvoiceID == invoiceID);

                    if (invoice == null)
                    {
                        MessageBox.Show("Invoice not found.");
                        return;
                    }

                    invoice.InvoiceDate = dtpInvoiceDate.Value;
                    invoice.TotalAmount = totalAmount;
                    invoice.PaidAmount = paidAmount;
                    invoice.Status = GetStatusValue();
                    invoice.Note = txtNote.Text.Trim();

                    db.SaveChanges();

                    MessageBox.Show("Invoice updated successfully.");
                }
            }

            LoadGridData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtInvoiceID.Text.Trim() == "")
            {
                MessageBox.Show("Please select an invoice to delete.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Do you want to delete this invoice?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            int invoiceID = Convert.ToInt32(txtInvoiceID.Text);

            using (DataContext db = new DataContext())
            {
                tblInvoices invoice = db.Invoices
                    .FirstOrDefault(x => x.InvoiceID == invoiceID);

                if (invoice == null)
                {
                    MessageBox.Show("Invoice not found.");
                    return;
                }

                db.Invoices.Remove(invoice);
                db.SaveChanges();
            }

            MessageBox.Show("Invoice deleted successfully.");
            LoadGridData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}