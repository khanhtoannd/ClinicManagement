using System;
using System.Linq;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmMedicalRecordsLINQ : Form
    {
        private bool AddNew = false;

        public frmMedicalRecordsLINQ()
        {
            InitializeComponent();
        }

        private void setControl(bool check)
        {
            txtRecordID.Enabled = false;

            cbbAppointmentID.Enabled = check;

            txtPatientID.Enabled = false;
            txtDoctorID.Enabled = false;

            txtDiagnosis.Enabled = check;
            txtSymptoms.Enabled = check;
            txtTreatment.Enabled = check;
            txtConclusion.Enabled = check;
            dtpRecordDate.Enabled = check;

            btnSave.Enabled = check;
            btnCancel.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            btnExit.Enabled = !check;

            dgvMedicalRecords.Enabled = !check;
        }

        private void ClearInput()
        {
            txtRecordID.Clear();
            txtPatientID.Clear();
            txtDoctorID.Clear();

            if (cbbAppointmentID.Items.Count > 0)
            {
                cbbAppointmentID.SelectedIndex = 0;
            }

            txtDiagnosis.Clear();
            txtSymptoms.Clear();
            txtTreatment.Clear();
            txtConclusion.Clear();

            dtpRecordDate.Value = DateTime.Now;
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

        private void LoadPatientDoctorByAppointment()
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
                    txtDoctorID.Text = appointment.DoctorID.ToString();
                }
                else
                {
                    txtPatientID.Clear();
                    txtDoctorID.Clear();
                }
            }
        }

        private void LoadGridData()
        {
            using (DataContext db = new DataContext())
            {
                var data = from record in db.MedicalRecords
                           join ap in db.Appointments
                                on record.AppointmentID equals ap.AppointmentID
                           join p in db.Patients
                                on ap.PatientID equals p.PatientID
                           join d in db.Doctors
                                on ap.DoctorID equals d.DoctorID
                           select new
                           {
                               record.RecordID,
                               record.AppointmentID,
                               ap.PatientID,
                               ap.DoctorID,
                               PatientName = p.FullName,
                               DoctorName = d.DoctorName,
                               record.Diagnosis,
                               record.Symptoms,
                               record.Treatment,
                               record.Conclusion,
                               record.RecordDate
                           };

                dgvMedicalRecords.AutoGenerateColumns = false;
                dgvMedicalRecords.DataSource = data.ToList();
            }

            setControl(false);
        }

        private void frmMedicalRecordsLINQ_Load(object sender, EventArgs e)
        {
            dgvMedicalRecords.AutoGenerateColumns = false;
            dgvMedicalRecords.AllowUserToAddRows = false;
            dgvMedicalRecords.ReadOnly = true;
            dgvMedicalRecords.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMedicalRecords.MultiSelect = false;

            LoadAppointments();
            LoadGridData();
            LoadPatientDoctorByAppointment();
        }

        private void cbbAppointmentID_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPatientDoctorByAppointment();
        }

        private void dgvMedicalRecords_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dgvMedicalRecords.Rows[e.RowIndex].Cells["RecordID"].Value == null)
                return;

            txtRecordID.Text =
                dgvMedicalRecords.Rows[e.RowIndex].Cells["RecordID"].Value.ToString();

            cbbAppointmentID.SelectedValue =
                dgvMedicalRecords.Rows[e.RowIndex].Cells["AppointmentID"].Value;

            LoadPatientDoctorByAppointment();

            txtDiagnosis.Text =
                dgvMedicalRecords.Rows[e.RowIndex].Cells["Diagnosis"].Value?.ToString();

            txtSymptoms.Text =
                dgvMedicalRecords.Rows[e.RowIndex].Cells["Symptoms"].Value?.ToString();

            txtTreatment.Text =
                dgvMedicalRecords.Rows[e.RowIndex].Cells["Treatment"].Value?.ToString();

            txtConclusion.Text =
                dgvMedicalRecords.Rows[e.RowIndex].Cells["Conclusion"].Value?.ToString();

            if (dgvMedicalRecords.Rows[e.RowIndex].Cells["RecordDate"].Value != null)
            {
                dtpRecordDate.Value = Convert.ToDateTime(
                    dgvMedicalRecords.Rows[e.RowIndex].Cells["RecordDate"].Value
                );
            }
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
            if (txtRecordID.Text.Trim() == "")
            {
                MessageBox.Show("Please select a medical record to edit.");
                return;
            }

            AddNew = false;
            setControl(true);

            cbbAppointmentID.Enabled = false;
            txtDiagnosis.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setControl(false);

            if (dgvMedicalRecords.CurrentRow != null)
            {
                int rowIndex = dgvMedicalRecords.CurrentRow.Index;
                dgvMedicalRecords_CellEnter(
                    this,
                    new DataGridViewCellEventArgs(0, rowIndex)
                );
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

            if (txtDiagnosis.Text.Trim() == "")
            {
                MessageBox.Show("Diagnosis is required.");
                txtDiagnosis.Focus();
                return;
            }

            using (DataContext db = new DataContext())
            {
                if (AddNew)
                {
                    bool exists = db.MedicalRecords
                        .Any(x => x.AppointmentID == appointmentID);

                    if (exists)
                    {
                        MessageBox.Show("This appointment already has a medical record.");
                        return;
                    }

                    tblMedicalRecords newRecord = new tblMedicalRecords();

                    newRecord.AppointmentID = appointmentID;
                    newRecord.Diagnosis = txtDiagnosis.Text.Trim();
                    newRecord.Symptoms = txtSymptoms.Text.Trim();
                    newRecord.Treatment = txtTreatment.Text.Trim();
                    newRecord.Conclusion = txtConclusion.Text.Trim();
                    newRecord.RecordDate = dtpRecordDate.Value;

                    db.MedicalRecords.Add(newRecord);
                    db.SaveChanges();

                    MessageBox.Show("Medical record added successfully.");
                }
                else
                {
                    int recordID = Convert.ToInt32(txtRecordID.Text);

                    tblMedicalRecords record = db.MedicalRecords
                        .FirstOrDefault(x => x.RecordID == recordID);

                    if (record == null)
                    {
                        MessageBox.Show("Medical record not found.");
                        return;
                    }

                    record.Diagnosis = txtDiagnosis.Text.Trim();
                    record.Symptoms = txtSymptoms.Text.Trim();
                    record.Treatment = txtTreatment.Text.Trim();
                    record.Conclusion = txtConclusion.Text.Trim();
                    record.RecordDate = dtpRecordDate.Value;

                    db.SaveChanges();

                    MessageBox.Show("Medical record updated successfully.");
                }
            }

            LoadGridData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (txtRecordID.Text.Trim() == "")
            {
                MessageBox.Show("Please select a medical record to delete.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Do you want to delete this medical record?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.No)
                return;

            int recordID = Convert.ToInt32(txtRecordID.Text);

            using (DataContext db = new DataContext())
            {
                tblMedicalRecords record = db.MedicalRecords
                    .FirstOrDefault(x => x.RecordID == recordID);

                if (record == null)
                {
                    MessageBox.Show("Medical record not found.");
                    return;
                }

                db.MedicalRecords.Remove(record);
                db.SaveChanges();
            }

            MessageBox.Show("Medical record deleted successfully.");
            LoadGridData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}