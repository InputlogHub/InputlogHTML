using System.Data.Odbc;
using System.Configuration;

namespace WebApp.Util
{
    /// <summary>
    /// Class used for easy access to database.
    /// ASP's Entity Framework didn't seem to like MySQL.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// Executes a query with a result set
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static OdbcCommand Query(string query)
        {
            var connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString);
            return new OdbcCommand(query,connection);
        }

        /// <summary>
        /// Executes a query without results
        /// </summary>
        /// <param name="stmt"></param>
        public static void Statement(string stmt)
        {
            var connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString);
            using (var command = new OdbcCommand(stmt, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        /// <summary>
        /// Executes a query with a single-object result 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static object Get(string query)
        {
            var connection = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySQLConnStr"].ConnectionString);
            using (var command = new OdbcCommand(query, connection))
            {
                connection.Open();
                var result = command.ExecuteScalar();
                connection.Close();
                return result;
            }
        }
    }
}