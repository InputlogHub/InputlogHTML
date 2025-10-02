using System.Collections.Generic;
using WebApp.Util;
using MySql.Data.MySqlClient;

namespace WebApp.Models
{
    public class MailModel
    {
        public string Name { get; private set; }
        public string Content { get; private set; }

        public MailModel(string name, string content)
        {
            Name = name;
            Content = content;
        }

        /// <summary>
        ///  Update the content of the mail with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        public static void UpdateContent(string name, string content)
        {
            Database.Statement(
                "UPDATE mails SET content='" + MySqlHelper.EscapeString(content) +
                "' WHERE name='" + name + "'"
            );
        }

        /// <summary>
        /// Fetches the mail with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MailModel FetchByName(string name)
        {
            MailModel result = null;
            using (var command = Database.Query(
                "SELECT content FROM mails " +
                "WHERE name='" + name + "'" +
                " LIMIT 1")
                )
            {
                if (command == null) return null;
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var content = reader["content"].ToString();
                        result = new MailModel(name, content);
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// Fetches all mail names in the databases
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllNames()
        {
            List<string> result = new List<string>();
            using (var command = Database.Query(
                "SELECT name FROM mails")
                )
            {
                if (command == null) return null;
                command.Connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["name"].ToString();
                        result.Add(name);
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return result;
            }
        }
    }
}