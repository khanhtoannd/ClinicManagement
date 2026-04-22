using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ClinicManagement
{
    internal class DBServices
    {
        private SqlConnection conn;

        public DBServices()
        {
            string connStr = @"Data Source=LAPTOP-5QJSJ8QA\SQLEXPRESS;Initial Catalog=CLINICMANAGEMENT;Integrated Security=True";
            conn = new SqlConnection(connStr);
        }

        public DataTable GetData(string sql, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();

            try
            {
                SqlCommand cmd = new SqlCommand(sql, conn);

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return dt;
        }

        public void RunQuery(string sql, SqlParameter[] parameters = null)
        {
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    }
}