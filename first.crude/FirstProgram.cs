using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace first.crude
{
    public partial class FirstProgram : Form
    {
        //path of database
        string path = "data_table.db";
        string cs = @"URI=file:"+Application.StartupPath+ "\\data_table.db"; //data create debug folder

        SQLiteConnection conn;
        SQLiteCommand cmd;
        SQLiteDataReader dr;

        public FirstProgram()
        {
            InitializeComponent();
        }

        //show data in table
        private void data_show()
        {
            var conn = new SQLiteConnection(cs);
            conn.Open();
            string stm = "SELECT id,name FROM test";
            var cmd = new SQLiteCommand(stm,conn);
            dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                dataGridView1.Rows.Insert(0,dr.GetString(0), dr.GetString(1));
            }
        }

        //Create database and table 
        private void Create_db()
        {
            if (!System.IO.File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                using (var sqlite = new  SQLiteConnection(@"Data Source=" + path)) 
                {
                    sqlite.Open();
                    string sql = "create table test (id varchar(12), name varchar(20))";
                    SQLiteCommand command = new SQLiteCommand(sql,sqlite);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                Console.WriteLine("Database cannot create");
                return;
            }
        }

        private void ClearEdts()
        {
            edtId.Clear();
            edtName.Clear();
        }

        //insert data
        private void btnInsert_Click(object sender, EventArgs e)
        {
            var conn = new SQLiteConnection(cs);
            conn.Open();
            var cmd = new SQLiteCommand();

            try
            {
                cmd.CommandText = "INSERT INTO test (id,name) VALUES (@id,@name)";
                cmd.Parameters.AddWithValue("@id", edtId.Text);
                cmd.Parameters.AddWithValue("@name", edtName.Text);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                dataGridView1.Rows.Clear();
                data_show();
                ClearEdts();
            }
            catch (Exception)
            {
                Console.WriteLine("Não foi possível inserir os dados");
            }
            finally
            {
                conn.Close();
            }
        }

        //update 
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var conn = new SQLiteConnection(cs);
            conn.Open();
            var cmd = new SQLiteCommand(conn);

            try
            {
                cmd.CommandText = "UPDATE test SET name=@Name WHERE id=@Id";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("Id", edtId.Text);
                cmd.Parameters.AddWithValue("@Name", edtName.Text);
                cmd.ExecuteNonQuery();
                dataGridView1.Rows.Clear();
                data_show();
                ClearEdts();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi possível atualizar os dados. Erro: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }



        private void btnDelete_Click(object sender, EventArgs e)
        {
            var conn = new SQLiteConnection(cs);
            conn.Open();
            var cmd = new SQLiteCommand(conn);

            try
            {
                cmd.CommandText = "DELETE FROM test WHERE id =@id";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", edtId.Text);
                cmd.ExecuteNonQuery();
                dataGridView1.Rows.Clear();
                data_show();
                ClearEdts();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi possível excluir os dados. Erro: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                dataGridView1.CurrentRow.Selected = true;
                edtId.Text = dataGridView1.Rows[e.RowIndex].Cells["Id"].FormattedValue.ToString();
                edtName.Text = dataGridView1.Rows[e.RowIndex].Cells["Name"].FormattedValue.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Create_db();
            data_show();
        }
    }
}
