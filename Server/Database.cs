using System;
using System.Data.Odbc;
using System.Configuration;

namespace Server
{
    public class Database
    {
        public OdbcCommand Query(String query)
        {
            var connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString);
            return new OdbcCommand(query,connection);
        }

        public void Statement(String stmt)
        {
            var connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString);
            using (var command = new OdbcCommand(stmt, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public Object Get(String query)
        {
            var connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString);
            using (var command = new OdbcCommand(query, connection))
            {
                connection.Open();
                Object result = command.ExecuteScalar();
                connection.Close();
                return result;
            }
        }
    }
}