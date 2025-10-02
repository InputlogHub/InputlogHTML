using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InputLog.Core.Events.EyeTracking
{
	/// <summary>
	/// Interface implemented by the sub-part information of 
	/// EyeTrackParts.
	/// </summary>
	public interface ISubPart
	{
		/// <summary>
		/// Initialize the eyetrack part based on the data that has been read from the 
		/// tobii file.
		/// </summary>
		/// <param name="data">The data for this eyetrack part.</param>
		/// <param name="index">The index map, mapping the tobii information to its index</param>
		void Initialize(string[] data, Dictionary<string, int> index);

		/// <summary>
		/// Merge this subpart with the other subpart. Merging can only be done
		/// if the two subparts have the same type. An explicit check for this 
		/// would still be required in implementing classes.
		/// </summary>
		/// <param name="other">The other part used to merge data with this part. 
		/// The 'other' part is left unchanged, this parts data is altered, depending on
		/// the merge of the parts.</param>
		void Merge(ISubPart other);

		/// <summary>
		/// Returns whether the SubPart contains any data or whether it is
		/// empty. SubParts for required data will always contain data, yet optional
		/// subParts, or subParts that may have empty values for a sample, may not
		/// contain any data. (Such as Mouse, Keyboard,...)
		/// </summary>
		/// <returns>True if the Subpart contains data, false if it doesn't.</returns>
		bool ContainsData();

		/// <summary>
		/// Returns whether the SubPart allows the merging of data. This is possible
		/// for some Subparts, yet for other subparts the merging of data would make
		/// no sense at all. (StudioEvent, MouseEvent...)
		/// </summary>
		/// <returns>True if this SubPart supports the merging of data, false 
		/// if it does not.</returns>
		bool AllowsDataMerge();
	}
}
