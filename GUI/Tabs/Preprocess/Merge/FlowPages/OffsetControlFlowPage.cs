using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUI.Flow;
using InputLog.Core.Util.Matching;

namespace GUI.Tabs.Preprocess.Merge.FlowPages
{
	class OffsetControlFlowPage: AbstractFlowPage
	{
		#region private_members
		/// <summary>
		/// The specific control of this flow page.
		/// </summary>
		private OffsetControler SpecificControl { get { return (OffsetControler)Control; } }
		#endregion

		public OffsetControlFlowPage(string eventType)
			: base(
			new OffsetControler("The defined offset pushes " + eventType 
                + " events x milliseconds before their actual time of occurrence." +
			                    "\nThus hopefully placing them before inputlog events."), 
			false, 
			false,
			"Check the precalculated offset values to be used for merging and alter them if necessary."
			)
		{
		}

		/// <summary>
		/// Function gets called if the offsets have changed in the previous page.
		/// </summary>
		/// <param name="offsets"></param>
		public void HandleOffsetsChanged(Dictionary<IMatch<string>, int> offsets)
		{
			SpecificControl.SetOffsets(offsets);
		}

		/// <summary>
		/// Returns a list of user specified offset match that had active (selected) files.
		/// </summary>
		/// <returns>Pair of match and offset.</returns>
		public List<KeyValuePair<IMatch<string>, int>> GetOffsets()
		{
			return SpecificControl.GetOffsets();
		}

		#region processing_tasks
		protected override InputLog.Core.Util.Progress.ProcessTask GetPreprocessTask()
		{
			throw new NotImplementedException();
		}

		protected override InputLog.Core.Util.Progress.ProcessTask GetProcessTask()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
