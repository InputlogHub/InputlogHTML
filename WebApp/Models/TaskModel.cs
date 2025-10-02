using System;
using System.Collections.Generic;
using System.Data.Odbc;
using MySql.Data.MySqlClient;
using WebApp.Util;
using InputLog.Core.Util.Server;

namespace WebApp.Models
{
    /// <summary>
    /// GENERAL REMARK: THE DATABASE INTERACTION CODE
    /// IN THIS CLASS CONTAINS A LOT OF DUPLICATION
    /// ... because it didn't seem worth the effort to
    /// create an extensible method for fetching tasks
    /// under specific conditions
    /// </summary>
    public class TaskModel
    {

        private TaskModel(int id, string userID, string time, string guid, int status, bool downloaded = false)
        {
            ID = id;
            UserID = userID;
            Time = time;
            Guid = guid;
            Status = status;
            Downloaded = downloaded;
        }

        public int ID { get; private set; }
        public string UserID { get; private set; }
        public string Time { get; private set; }
        public string Guid { get; private set; }
        public int Status { get; private set; }
        public bool Downloaded { get; private set; }
        
        /// <summary>
        ///  Changes status for task with guid Guid to NewStatus
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="newStatus"></param>
        public static void ChangeStatus(string guid, int newStatus)
        {
            Database.Statement(
                "UPDATE tasks SET status=" + newStatus +
                " WHERE guid='" + MySqlHelper.EscapeString(guid) + "'"
                );
        }

        /// <summary>
        /// Changes status to QUEUED for each PENDING/DELAY task.
        /// Used to restart tasks after server crash/reboot
        /// </summary>
        public static void RestartPending()
        {
            Database.Statement(
                "UPDATE tasks SET status=" + TaskStatus.STATUS_UNSTARTED +
                " WHERE status=" + TaskStatus.STATUS_PENDING + " OR status=" + TaskStatus.STATUS_DELAY
                );
        }

        /// <summary>
        /// Marks the task with ID as downloaded
        /// </summary>
        /// <param name="id"></param>
        public static void MarkDownloaded(int id)
        {
            Database.Statement(
                "UPDATE tasks SET downloaded=1 " +
                " WHERE id='" + id + "'"
                );
        }

        /// <summary>
        /// Deletes the task with ID
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(int id)
        {
            Database.Statement(
                "DELETE FROM task_analyses" +
                " WHERE fkID_task=" + id
                );
            Database.Statement(
                "DELETE FROM tasks" +
                " WHERE id=" + id
                );
        }

        /// <summary>
        /// Returns the StatusString for a Task
        /// </summary>
        /// <returns></returns>
        public string StatusString()
        {
            return TaskStatus.StatusString(Status);
        }

        /// <summary>
        /// Inserts a new Task with the given UserID, Guid and Status
        /// (Time, Status and Downloaded have default values) Returns the ID of the inserted Task
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="guid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private static int Insert(string userID, string guid, int status)
        {
            Database.Statement(
                "INSERT INTO tasks(fkID_user, guid, status) " +
                "VALUES('" + MySqlHelper.EscapeString(userID) + "', '"
                + MySqlHelper.EscapeString(guid) + "'," + status + ")"
                );
            var aid = int.Parse(Database.Get("SELECT id FROM tasks WHERE guid='"
                                               + MySqlHelper.EscapeString(guid) + "'").ToString());
            return aid;
        }

        /// <summary>
        /// Inserts a new Task with the given UserID, Guid and Status
        /// (Time, Status and Downloaded have default values),
        /// and inserts a list of associated analyses. Returns the ID of the inserted Task
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="guid"></param>
        /// <param name="status"></param>
        /// <param name="analyses"></param>
        /// <returns></returns>
        public static int InsertWithAnalyses(string userID, string guid, int status, IEnumerable<string> analyses)
        {
            var tid = Insert(userID, guid, status);
            foreach (var an in analyses)
            {
                Database.Statement(
                    "INSERT INTO task_analyses(fkID_task, fkID_analysis) " +
                    "VALUES(" + tid + "," + an + ")"
                    );
            }
            return tid;
        }

        /// <summary>
        /// Fetches a task with a given ID and the analyses associated with it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static KeyValuePair<TaskModel, List<AnalysisModel>> GetByIDWithAnalyses(int id)
        {
            var ans = new List<AnalysisModel>();
            TaskModel task = null;
            using (var command = Database.Query(
                "SELECT tasks.fkID_user AS task_user, tasks.time AS task_time, " +
                "tasks.status AS task_status, tasks.guid AS task_guid, tasks.downloaded AS task_downloaded, " +
                "analyses.id AS analysis_id, analyses.name AS analysis_name, analyses.active AS analysis_active " +
                "FROM tasks JOIN task_analyses ON tasks.id = task_analyses.fkID_task " +
                "JOIN analyses ON analyses.id = task_analyses.fkID_analysis " +
                "WHERE tasks.id='" + id + "'")
                )
            {
                if (command == null) return new KeyValuePair<TaskModel, List<AnalysisModel>>();
                try
                {
                    command.Connection.Open();
                }
                catch (OdbcException e)
                {
                    PrintODBCException(e);
                }
                using (var reader = command.ExecuteReader())
                {
                    var first = true;
                    while (reader.Read())
                    {
                        if (first)
                        {
                            first = false;
                            task = ReadTask(id, reader);
                        }
                        var aid = int.Parse(reader["analysis_id"].ToString());
                        var aName = reader["analysis_name"].ToString();
                        var aActive = int.Parse(reader["analysis_active"].ToString()) == 1;
                        var an = new AnalysisModel(aid, aName, aActive);
                        ans.Add(an);
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return new KeyValuePair<TaskModel, List<AnalysisModel>>(task, ans);
            }
        }

        /// <summary>
        ///  Fetches all tasks and the analyses associated with them
        /// </summary>
        /// <returns></returns>
        public static Dictionary<TaskModel, List<AnalysisModel>> FetchAllWithAnalyses()
        {
            var result = new Dictionary<TaskModel, List<AnalysisModel>>();
            using (var command = Database.Query(
                "SELECT tasks.id AS task_id, tasks.fkID_user AS task_user, tasks.time AS task_time, " +
                "tasks.status AS task_status, tasks.guid AS task_guid, tasks.downloaded AS task_downloaded, " +
                "analyses.id AS analysis_id, analyses.name AS analysis_name, analyses.active AS analysis_active " +
                "FROM tasks JOIN task_analyses ON tasks.id = task_analyses.fkID_task " +
                "JOIN analyses ON analyses.id = task_analyses.fkID_analysis " +
                "ORDER BY tasks.time DESC")
                )
            {
                if (command == null) return result;
                try
                {
                    command.Connection.Open();
                }
                catch (OdbcException e)
                {
                    PrintODBCException(e);
                }
                using (var reader = command.ExecuteReader())
                {
                    var currentID = 0;
                    TaskModel currentTask = null;
                    while (reader.Read())
                    {
                        var id = int.Parse(reader["task_id"].ToString());
                        if (id != currentID)
                        {
                            currentID = id;
                            currentTask = ReadTask(id, reader);
                            result.Add(currentTask, new List<AnalysisModel>());
                        }
                        var aid = int.Parse(reader["analysis_id"].ToString());
                        var aName = reader["analysis_name"].ToString();
                        var aActive = int.Parse(reader["analysis_active"].ToString()) == 1;
                        var an = new AnalysisModel(aid, aName, aActive);
                        if (currentTask != null) result[currentTask].Add(an);
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return result;
            }
        }

        /// <summary>
        ///  Fetches all Tasks that are queued and their analyses.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Dictionary<TaskModel, List<AnalysisModel>> FetchQueued(int limit)
        {
            // Limit does not appear in query since JOIN ruins the count
            var result = new Dictionary<TaskModel, List<AnalysisModel>>();
            using (var command = Database.Query(
                "SELECT tasks.id AS task_id, tasks.fkID_user AS task_user, tasks.time AS task_time, " +
                "tasks.status AS task_status, tasks.guid AS task_guid, tasks.downloaded AS task_downloaded, " +
                "analyses.id AS analysis_id, analyses.name AS analysis_name, analyses.active AS analysis_active " +
                "FROM tasks JOIN task_analyses ON tasks.id = task_analyses.fkID_task " +
                "JOIN analyses ON analyses.id = task_analyses.fkID_analysis " +
                "WHERE status=" + TaskStatus.STATUS_UNSTARTED +
                " ORDER BY tasks.time ASC"
                )
                )
            {
                if (command == null) return result;
                try
                {
                    command.Connection.Open();
                }
                catch (OdbcException e)
                {
                    PrintODBCException(e);
                }
                using (var reader = command.ExecuteReader())
                {
                    var currentID = 0;
                    TaskModel currentTask = null;
                    var count = 0;
                    while (reader.Read() && count < limit)
                    {
                        var id = int.Parse(reader["task_id"].ToString());
                        if (id != currentID)
                        {
                            count++;
                            currentID = id;
                            currentTask = ReadTask(id, reader);
                            result.Add(currentTask, new List<AnalysisModel>());
                        }
                        var aid = int.Parse(reader["analysis_id"].ToString());
                        var aName = reader["analysis_name"].ToString();
                        var aActive = int.Parse(reader["analysis_active"].ToString()) == 1;
                        var an = new AnalysisModel(aid, aName, aActive);
                        if (currentTask != null) result[currentTask].Add(an);
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// Fetches all tasks created by a given user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static List<TaskModel> FetchAllByUserID(string userID)
        {
            return FetchByUserID(userID, 0);
        }

        /// <summary>
        /// Fetches tasks created by a given user
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static List<TaskModel> FetchByUserID(string userID, int limit)
        {
            var result = new List<TaskModel>();
            using (var command = Database.Query(
                "SELECT id, time, status, fkID_user, guid, downloaded " +
                "FROM tasks " +
                "WHERE fkID_user='" + MySqlHelper.EscapeString(userID) + "'" +
                " ORDER BY tasks.time DESC" +
                (limit > 0 ? (" LIMIT " + limit) : ("")))
                )
            {
                if (command == null) return result;
                try
                {
                    command.Connection.Open();
                }
                catch (OdbcException e)
                {
                    PrintODBCException(e);
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadTask(reader));
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// Fetches all tasks (without analyses)
        /// </summary>
        /// <returns></returns>
        public static List<TaskModel> FetchAll()
        {
            var result = new List<TaskModel>();
            using (var command = Database.Query(
                "SELECT id, time, status, guid, fkID_user, downloaded " +
                "FROM tasks " +
                "ORDER BY tasks.time DESC")
                )
            {
                if (command == null) return result;
                try
                {
                    command.Connection.Open();
                }
                catch (OdbcException e)
                {
                    PrintODBCException(e);
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadTask(reader));
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// Fetches the task with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TaskModel FetchByID(int id)
        {
            TaskModel result = null;
            using (var command = Database.Query(
                "SELECT id, time, status, guid, fkID_user, downloaded " +
                "FROM tasks " +
                "WHERE id=" + id)
                )
            {
                if (command == null) return null;
                try
                {
                    command.Connection.Open();
                }
                catch (OdbcException e)
                {
                    PrintODBCException(e);
                }
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = ReadTask(reader);
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// Fetches tasks that have been completed, but not downloaded yet
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TaskModel> FetchCompletedNotDownloaded()
        {
            var result = new List<TaskModel>();
            using (var command = Database.Query(
                "SELECT id, time, status, guid, fkID_user, downloaded " +
                "FROM tasks " +
                "WHERE (status=" + TaskStatus.STATUS_COMPLETED + "OR status=" +
                TaskStatus.STATUS_PARTIALLY_COMPLETED + ") AND downloaded=0 " +
                "ORDER BY tasks.time DESC")
                )
            {
                if (command == null) return result;
                try
                {
                    command.Connection.Open();
                }
                catch (OdbcException e)
                {
                    PrintODBCException(e);
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadTask(reader));
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// Fetches tasks that have been uploaded at least X days ago
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static IEnumerable<TaskModel> FetchOutdated(int x)
        {
            var result = new List<TaskModel>();
            using (var command = Database.Query(
                "SELECT id, time, status, guid, fkID_user, downloaded " +
                "FROM tasks " + "WHERE DATEDIFF(NOW(), time) > " + x) )
            {
                if (command == null) return result;
                try
                {
                    command.Connection.Open();
                }
                catch (OdbcException e)
                {
                    PrintODBCException(e);
                }
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadTask(reader));
                    }
                    reader.Close();
                }
                command.Connection.Close();
                return result;
            }
        }

        private static void PrintODBCException(OdbcException e)
        {
            string errorMessages = "";

            for (int i = 0; i < e.Errors.Count; i++)
            {
                errorMessages += "Index #" + i + "\n" +
                                 "Message: " + e.Errors[i].Message + "\n" +
                                 "NativeError: " + e.Errors[i].NativeError + "\n" +
                                 "Source: " + e.Errors[i].Source + "\n" +
                                 "SQL: " + e.Errors[i].SQLState + "\n";
            }
            Console.WriteLine("An ODBC exception occurred.\n" + errorMessages);
        }

        /// <summary>
        /// Changes status for tasks older than X days to cleaned
        /// </summary>
        /// <param name="x"></param>
        public static void MarkPruned(int x)
        {
            Database.Statement(
                "UPDATE tasks SET status=" + TaskStatus.STATUS_PRUNED +
                " WHERE DATEDIFF(NOW(), time) > " + x
                );
        }

        /// <summary>
        /// Returns an integer representing how many tasks precede
        ///  the task with the given ID in the queue
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetPositionInQueue(int id)
        {
            var query = "SELECT COUNT(id) FROM tasks WHERE status=" + TaskStatus.STATUS_UNSTARTED + " AND id < " + id;
            return int.Parse(Database.Get(query).ToString());
        }

        private static TaskModel ReadTask(OdbcDataReader reader)
        {
            var id = int.Parse(reader["id"].ToString());
            var time = reader["time"].ToString();
            var guid = reader["guid"].ToString();
            var userID = reader["fkID_user"].ToString();
            var status = int.Parse(reader["status"].ToString());
            var downloaded = int.Parse(reader["downloaded"].ToString()) == 1;
            return new TaskModel(id, userID, time, guid, status, downloaded);
        }

        private static TaskModel ReadTask(int id, OdbcDataReader reader)
        {
            var userID = reader["task_user"].ToString();
            var time = reader["task_time"].ToString();
            var guid = reader["task_guid"].ToString();
            var status = int.Parse(reader["task_status"].ToString());
            var downloaded = int.Parse(reader["task_downloaded"].ToString()) == 1;
            TaskModel task = new TaskModel(id, userID, time, guid, status, downloaded);
            return task;
        }

        public bool HasStarted()
        {
            return (Status == TaskStatus.STATUS_PENDING || 
                Status == TaskStatus.STATUS_COMPLETED || 
                Status == TaskStatus.STATUS_DELAY ||
                Status == TaskStatus.STATUS_FAILED ||
                Status == TaskStatus.STATUS_PARTIALLY_COMPLETED);
        }

        public void Queue()
        {
            ChangeStatus(Guid, TaskStatus.STATUS_UNSTARTED);
        }

        public bool Done()
        {
            return (Status == TaskStatus.STATUS_PRUNED || Status == TaskStatus.STATUS_COMPLETED);
        }

        public bool ResultAvailable()
        {
            return (Status == TaskStatus.STATUS_COMPLETED);
        }
    }
}