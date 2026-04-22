using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ClinicManagement
{
    public partial class frmUsers : Form
    {
        private DataContext db = new DataContext();
        private bool AddNew = false;

        public frmUsers()
        {
            InitializeComponent();
        }

        private void setControl(bool check)
        {
            txtUserID.Enabled = false;
            txtUsername.Enabled = check;
            txtFullName.Enabled = check;
            txtPassword.Enabled = check;
            txtPhone.Enabled = check;
            txtEmail.Enabled = check;
            cbbRole.Enabled = check;
            ckbStatus.Enabled = check;
            txtCreatedDate.Enabled = false;

            btnSave.Enabled = check;
            btnCancel.Enabled = check;

            btnAdd.Enabled = !check;
            btnEdit.Enabled = !check;
            btnDelete.Enabled = !check;
            btnExit.Enabled = !check;
            dgvUsers.Enabled = !check;
        }

        private string GetMD5(string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        private void LoadGridData()
        {
            var data = from u in db.Users
                       select new
                       {
                           u.UserID,
                           u.Username,
                           u.FullName,
                           u.Password,
                           u.Phone,
                           u.Email,
                           u.Role,
                           u.Status,
                           u.CreatedDate
                       };

            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.DataSource = data.ToList();
            setControl(false);
        }

        private void frmUsers_Load(object sender, EventArgs e)
        {
            dgvUsers.AutoGenerateColumns = false;
            dgvUsers.AllowUserToAddRows = false;

            cbbRole.Items.Clear();
            cbbRole.Items.Add("Admin");
            cbbRole.Items.Add("Staff");
            cbbRole.DropDownStyle = ComboBoxStyle.DropDownList;

            txtPassword.PasswordChar = '*';

            LoadGridData();
        }

        private void dgvUsers_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvUsers.Rows[e.RowIndex].Cells["UserID"].Value == null)
                return;

            txtUserID.Text = dgvUsers.Rows[e.RowIndex].Cells["UserID"].Value.ToString();
            txtUsername.Text = dgvUsers.Rows[e.RowIndex].Cells["Username"].Value?.ToString();
            txtFullName.Text = dgvUsers.Rows[e.RowIndex].Cells["FullName"].Value?.ToString();
            txtPassword.Text = dgvUsers.Rows[e.RowIndex].Cells["Password"].Value?.ToString();
            txtPhone.Text = dgvUsers.Rows[e.RowIndex].Cells["Phone"].Value?.ToString();
            txtEmail.Text = dgvUsers.Rows[e.RowIndex].Cells["Email"].Value?.ToString();
            cbbRole.Text = dgvUsers.Rows[e.RowIndex].Cells["Role"].Value?.ToString();

            if (dgvUsers.Rows[e.RowIndex].Cells["CreatedDate"].Value != null &&
                dgvUsers.Rows[e.RowIndex].Cells["CreatedDate"].Value.ToString() != "")
            {
                txtCreatedDate.Text = Convert.ToDateTime(
                    dgvUsers.Rows[e.RowIndex].Cells["CreatedDate"].Value
                ).ToString("dd/MM/yyyy");
            }

            bool status = false;
            if (dgvUsers.Rows[e.RowIndex].Cells["Status"].Value != null)
            {
                status = Convert.ToBoolean(dgvUsers.Rows[e.RowIndex].Cells["Status"].Value);
            }
            ckbStatus.Checked = status;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNew = true;
            setControl(true);

            txtUserID.Clear();
            txtUsername.Clear();
            txtFullName.Clear();
            txtPassword.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            cbbRole.SelectedIndex = -1;
            ckbStatus.Checked = true;
            txtCreatedDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            txtUsername.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (txtUserID.Text.Trim() == "") return;

            AddNew = false;
            setControl(true);
            txtUsername.Enabled = false;
            txtFullName.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            setControl(false);

            if (dgvUsers.CurrentRow != null)
            {
                int rowIndex = dgvUsers.CurrentRow.Index;
                dgvUsers_CellEnter(this, new DataGridViewCellEventArgs(0, rowIndex));
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;

            if (MessageBox.Show("Delete this record?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvUsers.CurrentRow.Cells["UserID"].Value);
                var userDelete = db.Users.SingleOrDefault(u => u.UserID == id);

                if (userDelete != null)
                {
                    db.Users.Remove(userDelete);
                    db.SaveChanges();
                    LoadGridData();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Trim() == "")
            {
                MessageBox.Show("Username is required.");
                txtUsername.Focus();
                return;
            }

            if (txtPassword.Text.Trim() == "")
            {
                MessageBox.Show("Password is required.");
                txtPassword.Focus();
                return;
            }

            if (AddNew)
            {
                bool exists = db.Users.Any(u => u.Username == txtUsername.Text.Trim());
                if (exists)
                {
                    MessageBox.Show("Username already exists.");
                    txtUsername.Focus();
                    return;
                }

                tblUsers user = new tblUsers()
                {
                    Username = txtUsername.Text.Trim(),
                    Password = GetMD5(txtPassword.Text.Trim()),
                    FullName = txtFullName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Role = cbbRole.Text.Trim(),
                    Status = ckbStatus.Checked,
                    CreatedDate = DateTime.Now
                };

                db.Users.Add(user);
                db.SaveChanges();
            }
            else
            {
                if (txtUserID.Text.Trim() == "") return;

                int id = Convert.ToInt32(txtUserID.Text.Trim());
                var userUpdate = db.Users.SingleOrDefault(u => u.UserID == id);

                if (userUpdate != null)
                {
                    userUpdate.Password = GetMD5(txtPassword.Text.Trim());
                    userUpdate.FullName = txtFullName.Text.Trim();
                    userUpdate.Phone = txtPhone.Text.Trim();
                    userUpdate.Email = txtEmail.Text.Trim();
                    userUpdate.Role = cbbRole.Text.Trim();
                    userUpdate.Status = ckbStatus.Checked;

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