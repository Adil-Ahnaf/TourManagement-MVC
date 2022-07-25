using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TourManagemantSystem
{
    public class DBHelper
    {
        public void InsertData(string ConnectionString, string sp, SqlParameter[] sqlParameters)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(sp, sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(sqlParameters);
                    sqlCommand.ExecuteNonQuery();
                }
                sqlConnection.Close();
            }
        }

        public DataTable GetData(string ConnectionString, string sp)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                DataTable myData = new DataTable();

                using (SqlCommand sqlCommand = new SqlCommand(sp, sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    
                    myData.Load(reader);
                }
                sqlConnection.Close();
                return myData;
            }
        }

        public DataTable GetSelectedData(string ConnectionString, string sp, SqlParameter[] sqlParameters)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                DataTable myData = new DataTable();

                using (SqlCommand sqlCommand = new SqlCommand(sp, sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(sqlParameters);
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    myData.Load(reader);
                }
                sqlConnection.Close();
                return myData;
            }
        }

        public void DeleteData(string ConnectionString, string sp, SqlParameter[] sqlParameters)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(sp, sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(sqlParameters);
                    sqlCommand.ExecuteNonQuery();

                }
                sqlConnection.Close();
            }
        }

        public int CheckDataExit(string ConnectionString, string sp, SqlParameter[] sqlParameters)
        {
            int Id = -1;
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(sp, sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddRange(sqlParameters);
                    var result = sqlCommand.ExecuteScalar();

                    if (result != null) 
                    {
                        int.TryParse(result.ToString(), out Id);
                    }

                }
                sqlConnection.Close();
                return Id;
            }
        }
    }
}
