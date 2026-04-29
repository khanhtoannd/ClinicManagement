using System;
using System.Linq;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmSpecialtiesLINQ : Form
    {
        private bool AddNew = false;

        public frmSpecialtiesLINQ()
        {
            InitializeComponent();
        }

        private void setControl(bool check)
        {
            txtSpecialtyID.Enabled = false;
            txtCreatedDate.Enabled = false;

            txtSpecialtyName.Enabled = check;
            txtDescription.Enabled = check;

            btnSave.Enabled = check;
            btnCancel.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            btnExit.Enabled = !check;

            dgvSpecialties.Enabled = !check;
        }

        private void ClearInput()
        {
            txtSpecialtyID.Clear();
            txtSpecialtyName.Clear();
            txtDescription.Clear();
            txtCreatedDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void LoadGridData()
        {
            using (DataContext db = new DataContext())
            {
                var data = db.Specialties
                    .Select(s => new
                    {
                        s.SpecialtyID,
                        s.SpecialtyName,
                        s.Description,
                        s.CreatedDate
                    })
                    .ToList();

                dgvSpecialties.AutoGenerateColumns = false;
                dgvSpecialties.DataSource = data;
            }

            setControl(false);
        }

        private void frmSpecialtiesLINQ_Load(object sender, EventArgs e)
        {
            dgvSpecialties.AutoGenerateColumns = false;
            dgvSpecialties.AllowUserToAddRows = false;
            dgvSpecialties.ReadOnly = true;
            dgvSpecialties.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSpecialties.MultiSelect = false;

            LoadGridData();
        }

        private void dgvSpecialties_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvSpecialties.Rows[e.RowIndex].Cells["SpecialtyID"].Value == null)
                return;

            txtSpecialtyID.Text =
                dgvSpecialties.Rows[e.RowIndex].Cells["SpecialtyID"].Value.ToString();

            txtSpecialtyName.Text =
                dgvSpecialties.Rows[e.RowIndex].Cells["SpecialtyName"].Value?.ToString();

            txtDescription.Text =
                dgvSpecialties.Rows[e.RowIndex].Cells["Description"].Value?.ToString();

            if (dgvSpecialties.Rows[e.RowIndex].Cells["CreatedDate"].Value != null)
            {
                txtCreatedDate.Text =
                    Convert.ToDateTime(dgvSpecialties.Rows[e.RowIndex].Cells["CreatedDate"].Value)
                    .ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);
            ClearInput();

            txtSpecialtyName.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (txtSpecialtyID.Text.Trim() == "")
            {
                MessageBox.Show("Please select a specialty to edit.");
                return;
            }

            AddNew = false;
            setControl(true);

            txtSpecialtyName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setControl(false);

            if (dgvSpecialties.CurrentRow != null)
            {
                int rowIndex = dgvSpecialties.CurrentRow.Index;
                dgvSpecialties_CellEnter(this, new DataGridViewCellEventArgs(0, rowIndex));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtSpecialtyName.Text.Trim() == "")
            {
                MessageBox.Show("Specialty name is required.");
                txtSpecialtyName.Focus();
                return;
            }

            using (DataContext db = new DataContext())
            {
                if (AddNew)
                {
                    bool exists = db.Specialties.Any(s =>
                        s.SpecialtyName == txtSpecialtyName.Text.Trim()
                    );

                    if (exists)
                    {
                        MessageBox.Show("This specialty already exists.");
                        txtSpecialtyName.Focus();
                        return;
                    }

                    tblSpecialties specialty = new tblSpecialties();

                    specialty.SpecialtyName = txtSpecialtyName.Text.Trim();
                    specialty.Description = txtDescription.Text.Trim();
                    specialty.CreatedDate = DateTime.Now;

                    db.Specialties.Add(specialty);
                    db.SaveChanges();

                    MessageBox.Show("Specialty added successfully.");
                }
                else
                {
                    int specialtyID = Convert.ToInt32(txtSpecialtyID.Text);

                    tblSpecialties specialty = db.Specialties
                        .FirstOrDefault(s => s.SpecialtyID == specialtyID);

                    if (specialty == null)
                    {
                        MessageBox.Show("Specialty not found.");
                        return;
                    }

                    specialty.SpecialtyName = txtSpecialtyName.Text.Trim();
                    specialty.Description = txtDescription.Text.Trim();

                    db.SaveChanges();

                    MessageBox.Show("Specialty updated successfully.");
                }
            }

            LoadGridData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtSpecialtyID.Text.Trim() == "")
            {
                MessageBox.Show("Please select a specialty to delete.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Do you want to delete this specialty?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            int specialtyID = Convert.ToInt32(txtSpecialtyID.Text);

            using (DataContext db = new DataContext())
            {
                tblSpecialties specialty = db.Specialties
                    .FirstOrDefault(s => s.SpecialtyID == specialtyID);

                if (specialty == null)
                {
                    MessageBox.Show("Specialty not found.");
                    return;
                }

                db.Specialties.Remove(specialty);
                db.SaveChanges();
            }

            MessageBox.Show("Specialty deleted successfully.");
            LoadGridData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}