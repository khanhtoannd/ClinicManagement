using System;
using System.Linq;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmAppointmentsLINQ : Form
    {
        DataContext db = new DataContext();
        bool AddNew = false;

        public frmAppointmentsLINQ()
        {
            InitializeComponent();
        }

        private void setControl(bool check)
        {
            txtAppointmentID.Enabled = false;
            cbbPatientID.Enabled = check;
            cbbDoctorID.Enabled = check;
            dtpAppointmentDate.Enabled = check;
            txtReason.Enabled = check;
            cbbStatus.Enabled = check;
            txtNote.Enabled = check;

            btnSave.Enabled = check;
            btnCancel.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            btnExit.Enabled = !check;
            dgvAppointments.Enabled = !check;
        }

        private void LoadPatients()
        {
            var data = db.Patients
                .Select(p => new
                {
                    p.PatientID,
                    p.FullName
                })
                .ToList();

            cbbPatientID.DataSource = data;
            cbbPatientID.DisplayMember = "FullName";
            cbbPatientID.ValueMember = "PatientID";
            cbbPatientID.SelectedIndex = -1;
        }

        private void LoadDoctors()
        {
            var data = db.Doctors
                .Select(d => new
                {
                    d.DoctorID,
                    d.DoctorName
                })
                .ToList();

            cbbDoctorID.DataSource = data;
            cbbDoctorID.DisplayMember = "DoctorName";
            cbbDoctorID.ValueMember = "DoctorID";
            cbbDoctorID.SelectedIndex = -1;
        }

        private void LoadStatus()
        {
            cbbStatus.Items.Clear();
            cbbStatus.Items.Add("0");
            cbbStatus.Items.Add("1");
            cbbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cbbStatus.SelectedIndex = -1;
        }

        private void LoadGridData()
        {
            var data = from a in db.Appointments
                       select a;

            dgvAppointments.AutoGenerateColumns = false;
            dgvAppointments.DataSource = data.ToList();
            setControl(false);
        }

        private void frmAppointmentsLINQ_Load(object sender, EventArgs e)
        {
            dgvAppointments.AutoGenerateColumns = false;
            dgvAppointments.AllowUserToAddRows = false;
            dgvAppointments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAppointments.MultiSelect = false;

            LoadPatients();
            LoadDoctors();
            LoadStatus();
            LoadGridData();
        }

        private void dgvAppointments_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvAppointments.Rows[e.RowIndex].Cells["AppointmentID"].Value != null)
            {
                txtAppointmentID.Text = dgvAppointments.Rows[e.RowIndex].Cells["AppointmentID"].Value.ToString();

                if (dgvAppointments.Rows[e.RowIndex].Cells["PatientID"].Value != null &&
                    dgvAppointments.Rows[e.RowIndex].Cells["PatientID"].Value.ToString() != "")
                {
                    cbbPatientID.SelectedValue = Convert.ToInt32(
                        dgvAppointments.Rows[e.RowIndex].Cells["PatientID"].Value
                    );
                }

                if (dgvAppointments.Rows[e.RowIndex].Cells["DoctorID"].Value != null &&
                    dgvAppointments.Rows[e.RowIndex].Cells["DoctorID"].Value.ToString() != "")
                {
                    cbbDoctorID.SelectedValue = Convert.ToInt32(
                        dgvAppointments.Rows[e.RowIndex].Cells["DoctorID"].Value
                    );
                }

                if (dgvAppointments.Rows[e.RowIndex].Cells["AppointmentDate"].Value != null &&
                    dgvAppointments.Rows[e.RowIndex].Cells["AppointmentDate"].Value.ToString() != "")
                {
                    dtpAppointmentDate.Value = Convert.ToDateTime(
                        dgvAppointments.Rows[e.RowIndex].Cells["AppointmentDate"].Value
                    );
                }

                txtReason.Text = dgvAppointments.Rows[e.RowIndex].Cells["Reason"].Value?.ToString();
                cbbStatus.Text = dgvAppointments.Rows[e.RowIndex].Cells["Status"].Value?.ToString();
                txtNote.Text = dgvAppointments.Rows[e.RowIndex].Cells["Note"].Value?.ToString();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);

            txtAppointmentID.Clear();
            cbbPatientID.SelectedIndex = -1;
            cbbDoctorID.SelectedIndex = -1;
            dtpAppointmentDate.Value = DateTime.Now;
            txtReason.Clear();
            cbbStatus.SelectedIndex = -1;
            txtNote.Clear();

            cbbPatientID.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (txtAppointmentID.Text.Trim() == "") return;

            AddNew = false;
            setControl(true);
            cbbPatientID.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setControl(false);

            if (dgvAppointments.CurrentRow != null)
            {
                int rowIndex = dgvAppointments.CurrentRow.Index;
                dgvAppointments_CellEnter(this, new DataGridViewCellEventArgs(0, rowIndex));
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null) return;

            if (MessageBox.Show("Delete this appointment?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = int.Parse(dgvAppointments.CurrentRow.Cells["AppointmentID"].Value.ToString());
                var appointmentDelete = db.Appointments.SingleOrDefault(a => a.AppointmentID == id);

                if (appointmentDelete != null)
                {
                    db.Appointments.Remove(appointmentDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbbPatientID.SelectedValue == null)
            {
                MessageBox.Show("Patient is required.");
                cbbPatientID.Focus();
                return;
            }

            if (cbbDoctorID.SelectedValue == null)
            {
                MessageBox.Show("Doctor is required.");
                cbbDoctorID.Focus();
                return;
            }

            int statusValue = 1;
            int.TryParse(cbbStatus.Text.Trim(), out statusValue);

            if (AddNew)
            {
                tblAppointments newAppointment = new tblAppointments
                {
                    PatientID = Convert.ToInt32(cbbPatientID.SelectedValue),
                    DoctorID = Convert.ToInt32(cbbDoctorID.SelectedValue),
                    AppointmentDate = dtpAppointmentDate.Value,
                    Reason = txtReason.Text.Trim(),
                    Status = statusValue,
                    Note = txtNote.Text.Trim(),
                    CreatedDate = DateTime.Now
                };

                db.Appointments.Add(newAppointment);
                db.SaveChanges();
            }
            else
            {
                if (txtAppointmentID.Text.Trim() == "") return;

                int id = int.Parse(txtAppointmentID.Text.Trim());
                var appointmentUpdate = db.Appointments.SingleOrDefault(a => a.AppointmentID == id);

                if (appointmentUpdate != null)
                {
                    appointmentUpdate.PatientID = Convert.ToInt32(cbbPatientID.SelectedValue);
                    appointmentUpdate.DoctorID = Convert.ToInt32(cbbDoctorID.SelectedValue);
                    appointmentUpdate.AppointmentDate = dtpAppointmentDate.Value;
                    appointmentUpdate.Reason = txtReason.Text.Trim();
                    appointmentUpdate.Status = statusValue;
                    appointmentUpdate.Note = txtNote.Text.Trim();

                    db.SaveChanges();
                }
            }

            LoadGridData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void txtReason_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}