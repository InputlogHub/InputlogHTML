using System.Collections.Generic;
using WebApp.Util;
using MySql.Data.MySqlClient;

namespace WebApp.Models
{
    public class AnalysisModel
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public bool Active { get; private set; }

        public AnalysisModel(int id, string name, bool active)
        {
            ID = id;
            Name = name;
            Active = active;
        }

        /// <summary>
        /// Inserts a new analysis with the given Name and status active
        /// </summary>
        /// <param name="name"></param>
        public static void Insert(string name)
        {
            Database.Statement(
                "INSERT INTO analyses(name, active) " +
                "VALUES('" + MySqlHelper.EscapeString(name) + "', 1)"
            );
        }

        public static void Deactivate(int id)
        {
            UpdateActive(id, 0);
        }

        public static void Activate(int id)
        {
            UpdateActive(id, 1);
        }

        /// <summary>
        ///  Set the active-status of the Analysis with given ID to NewActive
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newActive"></param>
        private static void UpdateActive(int id, int newActive)
        {
            Database.Statement(
                "UPDATE analyses SET active=" + newActive +
                " WHERE id='" + id + "'"
            );
        }

        /// <summary>
        /// Fetches all analyzes
        /// </summary>
        /// <returns></returns>
        public static List<AnalysisModel> FetchAll()
        {
            var result = new List<AnalysisModel>();
            using (var command = Database.Query("SELECT id, name, active FROM analyses ORDER BY id ASC"))
            {
                if (command == null) return result;
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = int.Parse(reader["id"].ToString());
                        var name = reader["name"].ToString();
                        var active = int.Parse(reader["active"].ToString()) == 1;
                        result.Add(new AnalysisModel(id, name, active));
                    }
                    reader.Close();
                }
                command.Connection.Close();
            }
            return result;
        }

        /// <summary>
        /// Fetches the analysis with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AnalysisModel FetchByID(int id)
        {
            AnalysisModel result = null;
            using (var command = Database.Query(
                "SELECT id, name, active FROM analyses " +
                "WHERE id=" + id +
                " LIMIT 1")
                )
            {
                if (command == null) return null;
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var name = reader["name"].ToString();
                        var active = int.Parse(reader["active"].ToString()) == 1;
                        result = new AnalysisModel(id, name, active);
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return result;
            }
        }
    }
}