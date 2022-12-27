using System.Data;
using System.Data.SqlClient;

namespace WinFormsDBManager
{
    public partial class Form1 : Form
    {
        DataSet ds;
        SqlDataAdapter adapter;
        string connectionString = @"Server=.\SQLEXPRESS;Database=userdb;Trusted_Connection=True";
        string sql = "SELECT * FROM Users";

        public Form1()
        {
            InitializeComponent();

            dgvMain.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMain.AllowUserToAddRows = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter(sql, connection);

                ds = new DataSet();
                adapter.Fill(ds);
                dgvMain.DataSource = ds.Tables[0];
                dgvMain.Columns["Id"].ReadOnly = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables[0].NewRow();
            ds.Tables[0].Rows.Add(row);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                adapter = new SqlDataAdapter(sql, connection);
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);

                adapter.InsertCommand = new SqlCommand("sp_CreateUser", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 50, "Name"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@age", SqlDbType.Int, 0, "Age"));

                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@Id", SqlDbType.Int, 0, "Id");
                parameter.Direction = ParameterDirection.Output;

                adapter.Update(ds);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvMain.SelectedRows)
            {
                dgvMain.Rows.Remove(row);
            }
        }
    }
}