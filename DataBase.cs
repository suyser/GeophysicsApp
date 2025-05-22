using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace GeophysicsApp
{
    public class DataBase
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        public void OpenConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }
        public void CloseConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }
        public SqlConnection GetConnection()
        {
            return sqlConnection;
        }
        public DataTable SqlSelect(String s)
        {
            OpenConnection();
            SqlCommand sqlCommand = new SqlCommand(s);
            sqlCommand.Connection = GetConnection();
            SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            CloseConnection();
            return dt;
        }
        public void SqlQuery(String s)
        {
            OpenConnection();
            SqlCommand sqlCommand = new SqlCommand(s);
            sqlCommand.Connection = GetConnection();
            sqlCommand.ExecuteNonQuery();
            CloseConnection();
        }
    }
}
