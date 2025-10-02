using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WebApp.Models
{
    public class TaskDetailsViewModel
    {
        public TaskModel Task { get; private set; }
        public List<AnalysisModel> Analyses { get; private set; }
        public string OutputPath { get; private set; } /* Used for results link */

        public TaskDetailsViewModel(TaskModel task, List<AnalysisModel> analyses, String outputPath)
        {
            Task = task;
            Analyses = analyses;
            OutputPath = outputPath;
        }
    }

    public class TaskIndexViewModel
    {
        public List<TaskModel> Tasks { get; private set; }

        public TaskIndexViewModel(List<TaskModel> tasks)
        {
            Tasks = tasks;
        }
    }

    public class TaskCreateViewModel
    {
        public List<AnalysisModel> Analyses { get; private set; }

        public TaskCreateViewModel(List<AnalysisModel> analyses)
        {
            Analyses = analyses;
        }
    }

    public class TaskSaveViewModel
    {
        [Required]
        public HttpPostedFile File { get; set; }
    }
}