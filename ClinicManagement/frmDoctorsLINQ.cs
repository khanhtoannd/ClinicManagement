using System;
using System.Linq;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmDoctorsLINQ : Form
    {
        DataContext db = new DataContext();
        bool AddNew = false;

        public frmDoctorsLINQ()
        {
            InitializeComponent();
        }

        private void setControl(bool check)
        {
            txtDoctorID.Enabled = false;
            txtDoctorName.Enabled = check;
            cbbSpecialty.Enabled = check;
            txtPhone.Enabled = check;
            txtEmail.Enabled = check;
            txtQualification.Enabled = check;
            txtRoomNumber.Enabled = check;
            txtCreatedDate.Enabled = false;
            ckbStatus.Enabled = check;

            btnSave.Enabled = check;
            btnCancel.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            btnExit.Enabled = !check;
            dgvDoctors.Enabled = !check;
        }

        private void LoadSpecialty()
        {
            var data = db.Specialties
                         .Select(s => new
                         {
                             s.SpecialtyID,
                             s.SpecialtyName
                         })
                         .ToList();

            cbbSpecialty.DataSource = data;
            cbbSpecialty.DisplayMember = "SpecialtyName";
            cbbSpecialty.ValueMember = "SpecialtyID";
            cbbSpecialty.SelectedIndex = -1;
        }

        private void LoadGridData()
        {
            var data = (from d in db.Doctors
                        join s in db.Specialties on d.SpecialtyID equals s.SpecialtyID into ds
                        from s in ds.DefaultIfEmpty()
                        select new
                        {
                            d.DoctorID,
                            d.DoctorName,
                            Specialty = s != null ? s.SpecialtyName : "",
                            d.Phone,
                            d.Email,
                            d.Qualification,
                            d.RoomNumber,
                            d.CreatedDate,
                            d.Status
                        })
                        .ToList()
                        .Select(x => new
                        {
                            x.DoctorID,
                            x.DoctorName,
                            x.Specialty,
                            x.Phone,
                            x.Email,
                            x.Qualification,
                            x.RoomNumber,
                            CreatedDate = DateTime.Now,
                            x.Status
                        })
                        .ToList();

            dgvDoctors.AutoGenerateColumns = false;
            dgvDoctors.DataSource = data;
            setControl(false);
        }

        private void frmDoctorsLINQ_Load(object sender, EventArgs e)
        {
            dgvDoctors.AutoGenerateColumns = false;
            dgvDoctors.AllowUserToAddRows = false;
            LoadSpecialty();
            LoadGridData();
        }

        private void dgvDoctors_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDoctors.Rows[e.RowIndex].Cells["DoctorID"].Value != null)
            {
                txtDoctorID.Text = dgvDoctors.Rows[e.RowIndex].Cells["DoctorID"].Value.ToString();
                txtDoctorName.Text = dgvDoctors.Rows[e.RowIndex].Cells["DoctorName"].Value?.ToString();
                txtPhone.Text = dgvDoctors.Rows[e.RowIndex].Cells["Phone"].Value?.ToString();
                txtEmail.Text = dgvDoctors.Rows[e.RowIndex].Cells["Email"].Value?.ToString();
                txtQualification.Text = dgvDoctors.Rows[e.RowIndex].Cells["Qualification"].Value?.ToString();
                txtRoomNumber.Text = dgvDoctors.Rows[e.RowIndex].Cells["RoomNumber"].Value?.ToString();

                txtCreatedDate.Text = "";

                string specialtyName = dgvDoctors.Rows[e.RowIndex].Cells["Specialty"].Value?.ToString();
                if (!string.IsNullOrEmpty(specialtyName))
                    cbbSpecialty.Text = specialtyName;
                else
                    cbbSpecialty.SelectedIndex = -1;

                if (dgvDoctors.Rows[e.RowIndex].Cells["Status"].Value != null)
                {
                    ckbStatus.Checked = Convert.ToBoolean(
                        dgvDoctors.Rows[e.RowIndex].Cells["Status"].Value
                    );
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);

            txtDoctorID.Clear();
            txtDoctorName.Clear();
            cbbSpecialty.SelectedIndex = -1;
            txtPhone.Clear();
            txtEmail.Clear();
            txtQualification.Clear();
            txtRoomNumber.Clear();
            txtCreatedDate.Clear();
            ckbStatus.Checked = true;

            txtDoctorName.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (txtDoctorID.Text.Trim() == "") return;

            AddNew = false;
            setControl(true);
            txtDoctorName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setControl(false);

            if (dgvDoctors.CurrentRow != null)
            {
                int rowIndex = dgvDoctors.CurrentRow.Index;
                dgvDoctors_CellEnter(this, new DataGridViewCellEventArgs(0, rowIndex));
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDoctors.CurrentRow == null) return;

            if (MessageBox.Show("Delete this doctor?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = int.Parse(dgvDoctors.CurrentRow.Cells["DoctorID"].Value.ToString());
                var doctorDelete = db.Doctors.SingleOrDefault(d => d.DoctorID == id);

                if (doctorDelete != null)
                {
                    db.Doctors.Remove(doctorDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtDoctorName.Text.Trim() == "")
            {
                MessageBox.Show("Doctor name is required.");
                txtDoctorName.Focus();
                return;
            }

            int? specialtyId = null;
            if (cbbSpecialty.SelectedValue != null)
            {
                int tempId;
                if (int.TryParse(cbbSpecialty.SelectedValue.ToString(), out tempId))
                    specialtyId = tempId;
            }

            if (AddNew)
            {
                tblDoctors newDoctor = new tblDoctors
                {
                    DoctorName = txtDoctorName.Text.Trim(),
                    SpecialtyID = specialtyId,
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Qualification = txtQualification.Text.Trim(),
                    RoomNumber = txtRoomNumber.Text.Trim(),
                    Status = ckbStatus.Checked
                };

                db.Doctors.Add(newDoctor);
                db.SaveChanges();
            }
            else
            {
                if (txtDoctorID.Text.Trim() == "") return;

                int id = int.Parse(txtDoctorID.Text.Trim());
                var doctorUpdate = db.Doctors.SingleOrDefault(d => d.DoctorID == id);

                if (doctorUpdate != null)
                {
                    doctorUpdate.DoctorName = txtDoctorName.Text.Trim();
                    doctorUpdate.SpecialtyID = specialtyId;
                    doctorUpdate.Phone = txtPhone.Text.Trim();
                    doctorUpdate.Email = txtEmail.Text.Trim();
                    doctorUpdate.Qualification = txtQualification.Text.Trim();
                    doctorUpdate.RoomNumber = txtRoomNumber.Text.Trim();
                    doctorUpdate.Status = ckbStatus.Checked;

                    db.SaveChanges();
                }
            }

            LoadGridData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}