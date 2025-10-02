using System;
using System.Collections.Generic;
using System.Linq;
using WebApp.Models;
using System.Timers;
using InputLog.Core.Util.Server;

namespace Server
{
    /// <summary>
    /// Creates and manages TaskExecutors for Tasks in queue.
    /// </summary>
    class TaskManager
    {
        private const int NR_TASKS = 8;
        private int RunningTasksCount;
        private readonly List<TaskExecutor> RunningTasks;
        private static Timer _timer;
        const int CHECK_INTERVAL = 1;

        static int CalculateTimerInterval(int minute)
        {
            DateTime now = DateTime.Now;
            DateTime future = now.AddMinutes((minute - (now.Minute % minute))).
                AddSeconds(now.Second * -1).AddMilliseconds(now.Millisecond * -1);
            TimeSpan interval = future - now;
            return (int)interval.TotalMilliseconds;
        }

        // Actual cleanup of results is done by WebApp
        static void Cleanup(object sender, EventArgs e)
        {
            _timer.Interval = CalculateTimerInterval(CHECK_INTERVAL);
        }

        public TaskManager()
        {
            RunningTasksCount = 0;
            RunningTasks = new List<TaskExecutor>();
        }

        public void StartServer()
        {
            TaskModel.RestartPending();
            _timer = new Timer {Interval = CalculateTimerInterval(CHECK_INTERVAL)};
            _timer.Elapsed += Cleanup;
            _timer.Start();
            while (true)
            {
                if (RunningTasksCount > 0)
                {
                    CheckRunningTasks();
                }
                if (RunningTasksCount < NR_TASKS)
                {
                    StartNewTasks();
                }
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(15));
            }
        }

        private void StartNewTasks()
        {
            int nrNewTasks = NR_TASKS - RunningTasksCount;
            Dictionary<TaskModel, List<AnalysisModel>> newTasks = TaskModel.FetchQueued(nrNewTasks);
            foreach (KeyValuePair<TaskModel, List<AnalysisModel>> pair in newTasks)
            {
                TaskModel.ChangeStatus(pair.Key.Guid, TaskStatus.STATUS_PENDING);
                TaskExecutor exec = new TaskExecutor(new TaskWrapper(pair.Key, pair.Value));
                RunningTasks.Add(exec);
                RunningTasksCount++;
                Console.WriteLine("Running task [GUID: " + pair.Key.Guid + "]");
                exec.Start();
            }
        }

        private void CheckRunningTasks()
        {
            bool down = false;
            for (int i = RunningTasks.Count - 1; i >= 0; i--)
            {
                TaskExecutor te = RunningTasks.ElementAt(i);
                if (te.IsFinished())
                {
                    te.Stop();

                    // Rerun only once, if task was not successful.
                    if (te.Result.Result != TaskResult.ResCode.COMPLETED &&
                        !te.IsRerun)
                    {
                        Console.WriteLine("Rerunning task [GUID: " + te.Task.GetTaskGuid() + "]");
                        te.IsRerun = true;
                        te.Start();
                    }
                    else
                    {
                        RunningTasksCount--;
                        RunningTasks.RemoveAt(i);
                        var newStatus = InterpretTaskResult(te.Result);
                        TaskModel.ChangeStatus(te.Result.Guid, newStatus);
                    }
                }
                else if (te.IsServerDown())
                {
                    down = true;
                }
            }
            if (down)
            {
                RunningTasks.ForEach(t => TaskModel.ChangeStatus(t.Result.Guid, TaskStatus.STATUS_DELAY));
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(600));
                RunningTasks.ForEach(t => t.DoRestart());
            }
        }

        private int InterpretTaskResult(TaskResult taskResult)
        {
            switch (taskResult.Result)
            {
                case TaskResult.ResCode.COMPLETED:
                    return TaskStatus.STATUS_COMPLETED;
                case TaskResult.ResCode.FAILED:
                    return TaskStatus.STATUS_FAILED;
                case TaskResult.ResCode.PARTIALLY_COMPLETED:
                    return TaskStatus.STATUS_PARTIALLY_COMPLETED;
                default:
                    return TaskStatus.STATUS_FAILED;
            }
        }

    }
}
