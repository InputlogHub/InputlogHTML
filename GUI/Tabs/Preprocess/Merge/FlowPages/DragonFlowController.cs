using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUI.Flow;
using InputLog.Core.Merging.Sources.ProcessTasks;

namespace GUI.Tabs.Preprocess.Merge.FlowPages
{
	/// <summary>
	/// Flow Controller for controls the flow for the eye tracker.
	/// This class makes sure that the data is correctly passed between pages, when required.
	/// </summary>
	class DragonFlowController: AbstractFlowController
	{
        public DragonFlowController() :
			base(new LinkedList<AbstractFlowPage>((new AbstractFlowPage[] { 
					new FileSelectAndMatchFlowPage<DragonMergeFileValidator>(".dat", @"(corr)|(cmds)\.dat$"),
					new OffsetControlFlowPage("Dragon"),	
					new ProcessControlFlowPage<DragonMergeTask>("Dragon"),
				}).ToList<AbstractFlowPage>())
			)
		{
            ((FileSelectAndMatchFlowPage<DragonMergeFileValidator>)(Pages.ElementAt(0))).OffsetsChanged += 
				((OffsetControlFlowPage)(Pages.ElementAt(1))).HandleOffsetsChanged;
            ((ProcessControlFlowPage<DragonMergeTask>)Pages.ElementAt(2)).SetOffsetPage((OffsetControlFlowPage)Pages.ElementAt(1));
		}
	}
}
