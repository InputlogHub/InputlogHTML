using System;
using System.Collections.Generic;
using GUI.Flow;
using InputLog.Core.Util.Progress;
using InputLog.Core.Util.Matching;
using InputLog.Core.Util;

namespace GUI.Tabs.Preprocess.Merge.FlowPages
{
	class ProcessControlFlowPage<TMt>: AbstractFlowPage
        where TMt : ProcessTask
	{
		/// <summary>
		/// The page determining the offests for merging.
		/// </summary>
		private OffsetControlFlowPage OffsetPage;
		private List<KeyValuePair<IMatch<string>, int>> _Offsets;

	    private List<KeyValuePair<IMatch<string>, int>> Offsets
		{
			get
			{
				return _Offsets;
			}
			set
			{
				_Offsets = value;
				var allFiles = new List<string>();
				_Offsets.ForEach(offsetPair => allFiles.AddRange(offsetPair.Key.SelectedItems()));
				RootFolder = StringUtils.FindCommonPath(allFiles);
			}
		}

		private string RootFolder = "";

		private ProcessController _SpecificControl;
		private ProcessController SpecificControl
		{
			get { return _SpecificControl ?? (_SpecificControl = (ProcessController) Control); }
		}

		/// <summary>
		/// The instance of the process ttask to be executed by this page.
		/// </summary>
		private ProcessTask _ProcessTaskInstance;
		private readonly object _ProcessTaskLock = new object();

		public ProcessControlFlowPage(string type)
			: base(new ProcessController(),
			false,
			true,
			"Merging the Inputlog and " + type + " files.")
		{
		}

		/// <summary>
		/// Specify the offset page that provides the determined offsets.
		/// </summary>
		/// <param name="offsetPage"></param>
		public void SetOffsetPage(OffsetControlFlowPage offsetPage)
		{
			OffsetPage = offsetPage;
		}

		/// <summary>
		/// Handle the 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void HandleProcessProgress(object sender, ProgressEventArgs args)
		{
			if (args.Code != ProgressEventArgs.ProgressCode.FAILED)
			{
				SpecificControl.AppendTextTs(args.Message);
			}
			switch (args.Code)
			{
				case ProgressEventArgs.ProgressCode.DONE:
					lock (_ProcessTaskLock)
					{
						_ProcessTaskInstance = null;
					}
					SpecificControl.AlterOpenFolderLbl(RootFolder, true);
					break;
				case ProgressEventArgs.ProgressCode.FAILED:
					lock (_ProcessTaskLock)
					{
						_ProcessTaskInstance = null;
					}
					break;
				case ProgressEventArgs.ProgressCode.STARTED:
				case ProgressEventArgs.ProgressCode.STEP_COMPLETED:
				default:
					break;
			}
		}

		#region processing_tasks
		protected override ProcessTask GetPreprocessTask()
		{
			throw new NotImplementedException();
		}

		protected override ProcessTask GetProcessTask()
		{
			lock (_ProcessTaskLock)
			{
				if (_ProcessTaskInstance == null)
				{
					Offsets = OffsetPage.GetOffsets();
                    _ProcessTaskInstance = (TMt)Activator.CreateInstance(typeof(TMt),OffsetPage.GetOffsets());
					_ProcessTaskInstance.ProcessListeners += HandleProcessProgress;
				}
			}
			return _ProcessTaskInstance;
		}
		#endregion

	}
}
