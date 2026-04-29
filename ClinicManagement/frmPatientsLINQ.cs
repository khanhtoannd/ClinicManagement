using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace ClinicManagement
{
    public partial class frmPatientsLINQ : Form
    {
        DataContext db = new DataContext();
        bool AddNew = false;

        public frmPatientsLINQ()
        {
            InitializeComponent();
        }

        private void setControl(bool check)
        {
            txtPatientID.Enabled = false;
            txtFullName.Enabled = check;
            cbbGender.Enabled = check;
            dtpDateOfBirth.Enabled = check;
            txtPhone.Enabled = check;
            txtEmail.Enabled = check;
            txtAddress.Enabled = check;
            txtIdentityNumber.Enabled = check;
            txtNote.Enabled = check;
            txtCreatedDate.Enabled = false;

            btnSave.Enabled = check;
            btnCancel.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            btnExit.Enabled = !check;
            dgvPatients.Enabled = !check;
        }

        private void LoadGridData()
        {
            var data = from p in db.Patients
                       select p;

            dgvPatients.AutoGenerateColumns = false;
            dgvPatients.DataSource = data.ToList();
            setControl(false);
        }

        private void frmPatientsLINQ_Load(object sender, EventArgs e)
        {
            dgvPatients.AutoGenerateColumns = false;
            dgvPatients.AllowUserToAddRows = false;

            cbbGender.Items.Clear();
            cbbGender.Items.Add("Male");
            cbbGender.Items.Add("Female");
            cbbGender.Items.Add("Other");
            cbbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadGridData();
        }

        private void dgvPatients_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvPatients.Rows[e.RowIndex].Cells["PatientID"].Value != null)
            {
                txtPatientID.Text = dgvPatients.Rows[e.RowIndex].Cells["PatientID"].Value.ToString();
                txtFullName.Text = dgvPatients.Rows[e.RowIndex].Cells["FullName"].Value.ToString();
                cbbGender.Text = dgvPatients.Rows[e.RowIndex].Cells["Gender"].Value.ToString();

                if (dgvPatients.Rows[e.RowIndex].Cells["DateOfBirth"].Value != null &&
                    dgvPatients.Rows[e.RowIndex].Cells["DateOfBirth"].Value.ToString() != "")
                {
                    dtpDateOfBirth.Value = Convert.ToDateTime(
                        dgvPatients.Rows[e.RowIndex].Cells["DateOfBirth"].Value
                    );
                }

                txtPhone.Text = dgvPatients.Rows[e.RowIndex].Cells["Phone"].Value.ToString();
                txtEmail.Text = dgvPatients.Rows[e.RowIndex].Cells["Email"].Value.ToString();
                txtAddress.Text = dgvPatients.Rows[e.RowIndex].Cells["Address"].Value.ToString();
                txtIdentityNumber.Text = dgvPatients.Rows[e.RowIndex].Cells["IdentityNumber"].Value.ToString();
                txtNote.Text = dgvPatients.Rows[e.RowIndex].Cells["Note"].Value.ToString();

                if (dgvPatients.Rows[e.RowIndex].Cells["CreatedDate"].Value != null &&
                    dgvPatients.Rows[e.RowIndex].Cells["CreatedDate"].Value.ToString() != "")
                {
                    txtCreatedDate.Text = Convert.ToDateTime(
                        dgvPatients.Rows[e.RowIndex].Cells["CreatedDate"].Value
                    ).ToString("dd/MM/yyyy");
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);

            txtPatientID.Clear();
            txtFullName.Clear();
            cbbGender.SelectedIndex = -1;
            dtpDateOfBirth.Value = DateTime.Now;
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            txtIdentityNumber.Clear();
            txtNote.Clear();
            txtCreatedDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            txtFullName.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (txtPatientID.Text.Trim() == "") return;

            AddNew = false;
            setControl(true);
            txtFullName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setControl(false);

            if (dgvPatients.CurrentRow != null)
            {
                int rowIndex = dgvPatients.CurrentRow.Index;
                dgvPatients_CellEnter(this, new DataGridViewCellEventArgs(0, rowIndex));
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvPatients.CurrentRow == null) return;

            if (MessageBox.Show("Delete this patient?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = int.Parse(dgvPatients.CurrentRow.Cells["PatientID"].Value.ToString());
                var patientDelete = db.Patients.SingleOrDefault(p => p.PatientID == id);

                if (patientDelete != null)
                {
                    db.Patients.Remove(patientDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtFullName.Text.Trim() == "")
            {
                MessageBox.Show("Full name is required.");
                txtFullName.Focus();
                return;
            }

            if (AddNew)
            {
                tblPatients newPatient = new tblPatients
                {
                    FullName = txtFullName.Text.Trim(),
                    Gender = cbbGender.Text.Trim(),
                    DateOfBirth = dtpDateOfBirth.Value.Date,
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    IdentityNumber = txtIdentityNumber.Text.Trim(),
                    Note = txtNote.Text.Trim(),
                    CreatedDate = DateTime.Now
                };

                db.Patients.Add(newPatient);
                db.SaveChanges();
            }
            else
            {
                if (txtPatientID.Text.Trim() == "") return;

                int id = int.Parse(txtPatientID.Text.Trim());
                var patientUpdate = db.Patients.SingleOrDefault(p => p.PatientID == id);

                if (patientUpdate != null)
                {
                    patientUpdate.FullName = txtFullName.Text.Trim();
                    patientUpdate.Gender = cbbGender.Text.Trim();
                    patientUpdate.DateOfBirth = dtpDateOfBirth.Value.Date;
                    patientUpdate.Phone = txtPhone.Text.Trim();
                    patientUpdate.Email = txtEmail.Text.Trim();
                    patientUpdate.Address = txtAddress.Text.Trim();
                    patientUpdate.IdentityNumber = txtIdentityNumber.Text.Trim();
                    patientUpdate.Note = txtNote.Text.Trim();

                    db.SaveChanges();
                }
            }

            LoadGridData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }
    }
}