using System.Collections.Generic;
using InputLog.Core.Util.KeyConversion;
using InputLog.Core.Events;
using InputLog.Core.IO;

namespace InputLog.Core.Analyses.GeneralEyetrack
{
	/// <summary>
	/// Description of a general analysis that condenses the Eyetrack information in the file.
	/// </summary>
	public class GeneralEyetrackAnalysisDescription: General.GeneralAnalysisDescription
	{
		public GeneralEyetrackAnalysisDescription() : this("GEA") { }
		private GeneralEyetrackAnalysisDescription(string abbreviation) : base(abbreviation) { }

		public GeneralEyetrackAnalysisDescription(List<Event> events, SessionIdentification sessionID,
			string abbreviation, List<KeysEx> controlKeys = null):
			base(events, sessionID, abbreviation, controlKeys)
		{ }

		public override string GetName()
		{
			return "General Eyetrack Analysis";
		}

		public override Analysis GetAnalysis()
		{
			return new GeneralEyetrackAnalysis(Events, SessionID, Abbreviation, ControlKeys);
		}
	}
}
