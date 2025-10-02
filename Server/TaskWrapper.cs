using System.Collections.Generic;
using System.Linq;
using WebApp.Models;


namespace Server
{
    /// <summary>
    /// Provides a wrapper around the TaskModel, to decrease coupling between Server and WebApp.
    /// </summary>
    class TaskWrapper
    {
        private readonly TaskModel Task;
        private readonly List<AnalysisModel> Analyses;

        public TaskWrapper(TaskModel task, List<AnalysisModel> analyses)
        {
            Task = task;
            Analyses = analyses;
        }

        public string GetUserID()
        {
            return Task.UserID;
        }

        public string GetTaskGuid()
        {
            return Task.Guid;
        }

        public string GetAnalysis()
        {
            return Analyses.First().Name;
        }
    }
}
