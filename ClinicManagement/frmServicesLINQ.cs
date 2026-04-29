using System;
using System.Linq;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmServicesLINQ : Form
    {
        private bool AddNew = false;

        public frmServicesLINQ()
        {
            InitializeComponent();
        }

        private void setControl(bool check)
        {
            txtServiceID.Enabled = false;

            txtServiceName.Enabled = check;
            txtPrice.Enabled = check;
            txtDescription.Enabled = check;
            ckbStatus.Enabled = check;

            btnSave.Enabled = check;
            btnCancel.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            btnExit.Enabled = !check;

            dataGridView1.Enabled = !check;
        }

        private void ClearInput()
        {
            txtServiceID.Clear();
            txtServiceName.Clear();
            txtPrice.Text = "0";
            txtDescription.Clear();
            ckbStatus.Checked = true;
        }

        private void LoadGridData()
        {
            using (DataContext db = new DataContext())
            {
                var data = db.Services
                    .Select(s => new
                    {
                        s.ServiceID,
                        s.ServiceName,
                        s.Price,
                        s.Description,
                        StatusText = s.Status ? "Active" : "Inactive"
                    })
                    .ToList();

                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.DataSource = data;
            }

            setControl(false);
        }

        private void frmServicesLINQ_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            LoadGridData();
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dataGridView1.Rows[e.RowIndex].Cells["ServiceID"].Value == null)
                return;

            txtServiceID.Text =
                dataGridView1.Rows[e.RowIndex].Cells["ServiceID"].Value.ToString();

            txtServiceName.Text =
                dataGridView1.Rows[e.RowIndex].Cells["ServiceName"].Value?.ToString();

            txtPrice.Text =
                dataGridView1.Rows[e.RowIndex].Cells["Price"].Value?.ToString();

            txtDescription.Text =
                dataGridView1.Rows[e.RowIndex].Cells["Description"].Value?.ToString();

            if (dataGridView1.Rows[e.RowIndex].Cells["Status"].Value != null)
            {
                string statusText = dataGridView1.Rows[e.RowIndex].Cells["Status"].Value.ToString();

                ckbStatus.Checked = statusText == "Active";
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);
            ClearInput();

            txtServiceName.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (txtServiceID.Text.Trim() == "")
            {
                MessageBox.Show("Please select a service to edit.");
                return;
            }

            AddNew = false;
            setControl(true);

            txtServiceName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setControl(false);

            if (dataGridView1.CurrentRow != null)
            {
                int rowIndex = dataGridView1.CurrentRow.Index;
                dataGridView1_CellEnter(this, new DataGridViewCellEventArgs(0, rowIndex));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtServiceName.Text.Trim() == "")
            {
                MessageBox.Show("Service name is required.");
                txtServiceName.Focus();
                return;
            }

            decimal price;

            if (!decimal.TryParse(txtPrice.Text.Trim(), out price))
            {
                MessageBox.Show("Price is invalid.");
                txtPrice.Focus();
                return;
            }

            using (DataContext db = new DataContext())
            {
                if (AddNew)
                {
                    tblServices service = new tblServices();

                    service.ServiceName = txtServiceName.Text.Trim();
                    service.Price = price;
                    service.Description = txtDescription.Text.Trim();
                    service.Status = ckbStatus.Checked;

                    db.Services.Add(service);
                    db.SaveChanges();

                    MessageBox.Show("Service added successfully.");
                }
                else
                {
                    int serviceID = Convert.ToInt32(txtServiceID.Text);

                    tblServices service = db.Services
                        .FirstOrDefault(s => s.ServiceID == serviceID);

                    if (service == null)
                    {
                        MessageBox.Show("Service not found.");
                        return;
                    }

                    service.ServiceName = txtServiceName.Text.Trim();
                    service.Price = price;
                    service.Description = txtDescription.Text.Trim();
                    service.Status = ckbStatus.Checked;

                    db.SaveChanges();

                    MessageBox.Show("Service updated successfully.");
                }
            }

            LoadGridData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtServiceID.Text.Trim() == "")
            {
                MessageBox.Show("Please select a service to delete.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Do you want to delete this service?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            int serviceID = Convert.ToInt32(txtServiceID.Text);

            using (DataContext db = new DataContext())
            {
                tblServices service = db.Services
                    .FirstOrDefault(s => s.ServiceID == serviceID);

                if (service == null)
                {
                    MessageBox.Show("Service not found.");
                    return;
                }

                db.Services.Remove(service);
                db.SaveChanges();
            }

            MessageBox.Show("Service deleted successfully.");
            LoadGridData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}