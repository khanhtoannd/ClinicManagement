using System;
using System.Linq;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmInvoiceDetailsLINQ : Form
    {
        private bool AddNew = false;

        public frmInvoiceDetailsLINQ()
        {
            InitializeComponent();
        }

        private void setControl(bool check)
        {
            txtInvoiceDetailID.Enabled = false;

            cbbInvoiceID.Enabled = check;
            cbbServiceID.Enabled = check;

            txtQuantity.Enabled = check;
            txtUnitPrice.Enabled = check;
            txtAmount.Enabled = false;
            txtNote.Enabled = check;

            btnSave.Enabled = check;
            btnCancel.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            btnExit.Enabled = !check;

            dgvInvoiceDetails.Enabled = !check;
        }

        private void ClearInput()
        {
            txtInvoiceDetailID.Clear();

            txtQuantity.Text = "1";
            txtUnitPrice.Text = "0";
            txtAmount.Text = "0";
            txtNote.Clear();

            if (cbbInvoiceID.Items.Count > 0)
                cbbInvoiceID.SelectedIndex = 0;

            if (cbbServiceID.Items.Count > 0)
                cbbServiceID.SelectedIndex = 0;
        }

        private bool TryGetInvoiceID(out int invoiceID)
        {
            invoiceID = 0;

            if (cbbInvoiceID.SelectedValue == null)
                return false;

            return int.TryParse(cbbInvoiceID.SelectedValue.ToString(), out invoiceID);
        }

        private bool TryGetServiceID(out int serviceID)
        {
            serviceID = 0;

            if (cbbServiceID.SelectedValue == null)
                return false;

            return int.TryParse(cbbServiceID.SelectedValue.ToString(), out serviceID);
        }

        private void LoadInvoices()
        {
            using (DataContext db = new DataContext())
            {
                var data = db.Invoices
                    .Select(i => new
                    {
                        i.InvoiceID,
                        DisplayText = "Invoice " + i.InvoiceID
                    })
                    .ToList();

                cbbInvoiceID.DataSource = data;
                cbbInvoiceID.DisplayMember = "DisplayText";
                cbbInvoiceID.ValueMember = "InvoiceID";
            }
        }

        private void LoadServices()
        {
            using (DataContext db = new DataContext())
            {
                var data = db.Services
                    .Where(s => s.Status == true)
                    .Select(s => new
                    {
                        s.ServiceID,
                        DisplayText = s.ServiceName
                    })
                    .ToList();

                cbbServiceID.DataSource = data;
                cbbServiceID.DisplayMember = "DisplayText";
                cbbServiceID.ValueMember = "ServiceID";
            }
        }

        private void LoadUnitPriceByService()
        {
            int serviceID;

            if (!TryGetServiceID(out serviceID))
                return;

            using (DataContext db = new DataContext())
            {
                var service = db.Services
                    .FirstOrDefault(s => s.ServiceID == serviceID);

                if (service != null)
                {
                    txtUnitPrice.Text = service.Price.ToString();
                    CalculateAmount();
                }
            }
        }

        private void CalculateAmount()
        {
            int quantity;
            decimal unitPrice;

            if (!int.TryParse(txtQuantity.Text.Trim(), out quantity))
                quantity = 0;

            if (!decimal.TryParse(txtUnitPrice.Text.Trim(), out unitPrice))
                unitPrice = 0;

            decimal amount = quantity * unitPrice;
            txtAmount.Text = amount.ToString();
        }

        private void UpdateInvoiceTotal(int invoiceID)
        {
            using (DataContext db = new DataContext())
            {
                decimal total = db.InvoiceDetails
                    .Where(x => x.InvoiceID == invoiceID)
                    .Select(x => (decimal?)(x.Quantity * x.UnitPrice))
                    .Sum() ?? 0;

                tblInvoices invoice = db.Invoices
                    .FirstOrDefault(x => x.InvoiceID == invoiceID);

                if (invoice != null)
                {
                    invoice.TotalAmount = total;

                    if (invoice.PaidAmount >= invoice.TotalAmount && invoice.TotalAmount > 0)
                        invoice.Status = 1;
                    else
                        invoice.Status = 0;

                    db.SaveChanges();
                }
            }
        }

        private void LoadGridData()
        {
            using (DataContext db = new DataContext())
            {
                var data = from detail in db.InvoiceDetails
                           join service in db.Services
                                on detail.ServiceID equals service.ServiceID
                           select new
                           {
                               detail.InvoiceDetailID,
                               detail.InvoiceID,
                               detail.ServiceID,
                               service.ServiceName,
                               detail.Quantity,
                               detail.UnitPrice,
                               Amount = detail.Quantity * detail.UnitPrice,
                               detail.Note
                           };

                dgvInvoiceDetails.AutoGenerateColumns = false;
                dgvInvoiceDetails.DataSource = data.ToList();
            }

            setControl(false);
        }

        private void frmInvoiceDetailsLINQ_Load(object sender, EventArgs e)
        {
            dgvInvoiceDetails.AutoGenerateColumns = false;
            dgvInvoiceDetails.AllowUserToAddRows = false;
            dgvInvoiceDetails.ReadOnly = true;
            dgvInvoiceDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInvoiceDetails.MultiSelect = false;

            LoadInvoices();
            LoadServices();
            LoadGridData();
        }

        private void cbbServiceID_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUnitPriceByService();
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            CalculateAmount();
        }

        private void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {
            CalculateAmount();
        }

        private void dgvInvoiceDetails_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvInvoiceDetails.Rows[e.RowIndex].Cells["InvoiceDetailID"].Value == null)
                return;

            txtInvoiceDetailID.Text =
                dgvInvoiceDetails.Rows[e.RowIndex].Cells["InvoiceDetailID"].Value.ToString();

            cbbInvoiceID.SelectedValue =
                dgvInvoiceDetails.Rows[e.RowIndex].Cells["InvoiceID"].Value;

            cbbServiceID.SelectedValue =
                dgvInvoiceDetails.Rows[e.RowIndex].Cells["ServiceID"].Value;

            txtQuantity.Text =
                dgvInvoiceDetails.Rows[e.RowIndex].Cells["Quantity"].Value.ToString();

            txtUnitPrice.Text =
                dgvInvoiceDetails.Rows[e.RowIndex].Cells["UnitPrice"].Value.ToString();

            txtAmount.Text =
                dgvInvoiceDetails.Rows[e.RowIndex].Cells["Amount"].Value.ToString();

            txtNote.Text =
                dgvInvoiceDetails.Rows[e.RowIndex].Cells["Note"].Value?.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);
            ClearInput();

            cbbInvoiceID.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (txtInvoiceDetailID.Text.Trim() == "")
            {
                MessageBox.Show("Please select an invoice detail to edit.");
                return;
            }

            AddNew = false;
            setControl(true);

            cbbInvoiceID.Enabled = false;
            cbbServiceID.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setControl(false);

            if (dgvInvoiceDetails.CurrentRow != null)
            {
                int rowIndex = dgvInvoiceDetails.CurrentRow.Index;
                dgvInvoiceDetails_CellEnter(this, new DataGridViewCellEventArgs(0, rowIndex));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int invoiceID;
            int serviceID;
            int quantity;
            decimal unitPrice;

            if (!TryGetInvoiceID(out invoiceID))
            {
                MessageBox.Show("Please select an invoice.");
                return;
            }

            if (!TryGetServiceID(out serviceID))
            {
                MessageBox.Show("Please select a service.");
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out quantity) || quantity <= 0)
            {
                MessageBox.Show("Quantity is invalid.");
                txtQuantity.Focus();
                return;
            }

            if (!decimal.TryParse(txtUnitPrice.Text.Trim(), out unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Unit price is invalid.");
                txtUnitPrice.Focus();
                return;
            }

            using (DataContext db = new DataContext())
            {
                if (AddNew)
                {
                    tblInvoiceDetails detail = new tblInvoiceDetails();

                    detail.InvoiceID = invoiceID;
                    detail.ServiceID = serviceID;
                    detail.Quantity = quantity;
                    detail.UnitPrice = unitPrice;
                    detail.Note = txtNote.Text.Trim();

                    db.InvoiceDetails.Add(detail);
                    db.SaveChanges();

                    MessageBox.Show("Invoice detail added successfully.");
                }
                else
                {
                    int invoiceDetailID = Convert.ToInt32(txtInvoiceDetailID.Text);

                    tblInvoiceDetails detail = db.InvoiceDetails
                        .FirstOrDefault(x => x.InvoiceDetailID == invoiceDetailID);

                    if (detail == null)
                    {
                        MessageBox.Show("Invoice detail not found.");
                        return;
                    }

                    detail.ServiceID = serviceID;
                    detail.Quantity = quantity;
                    detail.UnitPrice = unitPrice;
                    detail.Note = txtNote.Text.Trim();

                    db.SaveChanges();

                    MessageBox.Show("Invoice detail updated successfully.");
                }
            }

            UpdateInvoiceTotal(invoiceID);
            LoadGridData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtInvoiceDetailID.Text.Trim() == "")
            {
                MessageBox.Show("Please select an invoice detail to delete.");
                return;
            }

            int invoiceID;

            if (!TryGetInvoiceID(out invoiceID))
            {
                MessageBox.Show("Please select an invoice.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Do you want to delete this invoice detail?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            int invoiceDetailID = Convert.ToInt32(txtInvoiceDetailID.Text);

            using (DataContext db = new DataContext())
            {
                tblInvoiceDetails detail = db.InvoiceDetails
                    .FirstOrDefault(x => x.InvoiceDetailID == invoiceDetailID);

                if (detail == null)
                {
                    MessageBox.Show("Invoice detail not found.");
                    return;
                }

                db.InvoiceDetails.Remove(detail);
                db.SaveChanges();
            }

            UpdateInvoiceTotal(invoiceID);

            MessageBox.Show("Invoice detail deleted successfully.");
            LoadGridData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}