using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using InputLog.Core.Util.Matching;

namespace InputLog.Core.Merging.Sources.ProcessTasks
{
	public static class OffsetCalculatorFactory
	{
        private static readonly string TOBII_EXT = ".tsv";
        private static readonly string DRAGON_EXT = ".dat";

		public static IOffsetCalculator Create(List<IMatch<string>> matches)
		{
			if (matches == null || matches.Count == 0)
			{
				throw new ArgumentNullException("matches", "Matches can not be null, you must specify at least one match.");
			}

			foreach (IMatch<string> match in matches)
			{
				if (match.SelectedItems().Count == 0)
				{
					continue;
				}

				if (match.SelectedItems().Any(file => Path.GetExtension(file) == TOBII_EXT))
                    return new TobiiOffsetCalculator(matches);

                if (match.SelectedItems().Any(file => Path.GetExtension(file) == DRAGON_EXT))
                    return new DragonOffsetCalculator(matches);

			}
            return null;
		}
	}
}
