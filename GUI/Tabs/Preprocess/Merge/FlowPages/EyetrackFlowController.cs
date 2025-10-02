using System.Collections.Generic;
using System.Linq;
using GUI.Flow;
using InputLog.Core.Merging.Sources.ProcessTasks;

namespace GUI.Tabs.Preprocess.Merge.FlowPages
{
	/// <summary>
	/// Flow Controller for controls the flow for the eye tracker.
	/// This class makes sure that the data is correctly passed between pages, when required.
	/// </summary>
	class EyetrackFlowController: AbstractFlowController
	{
		public EyetrackFlowController():
			base(new LinkedList<AbstractFlowPage>((new AbstractFlowPage[] { 
					new FileSelectAndMatchFlowPage<TobiiMergeFileValidator>(".tsv"),
					new OffsetControlFlowPage("Eyetrack"),	
					new ProcessControlFlowPage<TobiiMergeTask>("Eyetracking")
				}).ToList())
			)
		{
            ((FileSelectAndMatchFlowPage<TobiiMergeFileValidator>)(Pages.ElementAt(0))).OffsetsChanged += 
				((OffsetControlFlowPage)(Pages.ElementAt(1))).HandleOffsetsChanged;
            ((ProcessControlFlowPage<TobiiMergeTask>)Pages.ElementAt(2)).SetOffsetPage((OffsetControlFlowPage)Pages.ElementAt(1));
		}
	}
}
