using System.Xml;
using InputLog.Core.Events.DragonNS;
using Part = InputLog.Core.IO.Xml.XmlElements.Log.Events.Event.Part;

namespace InputLog.Core.IO.Xml.Output
{
    public class XmlDragonNSWriter
	{
		/// <summary>
		/// Write the eyetrack eventpart.
		/// </summary>
		/// <param name="e">The eventpart to write</param>
        public static void WriteDragonNSPart(DragonNSPart e, XmlWriter writer)
		{
			writer.WriteStartElement(Part.TAG);
            writer.WriteAttributeString(Part.ATTRIBUTES.Type.KEY, Part.ATTRIBUTES.Type.VALUES.DragonNS);
            writer.WriteElementString(Part.DragonNS.ID.TAG, e.ID.ToString());
            writer.WriteElementString(Part.DragonNS.TotalStartTime.TAG, e.TotalStartTime.ToString());
            writer.WriteElementString(Part.DragonNS.TotalEndTime.TAG, e.TotalEndTime.ToString());
            writer.WriteElementString(Part.DragonNS.StartTime.TAG, e.StartTime.ToString());
            writer.WriteElementString(Part.DragonNS.EndTime.TAG, e.EndTime.ToString());
            writer.WriteElementString(Part.DragonNS.Guid.TAG, e.Guid);
            writer.WriteElementString(Part.DragonNS.Text.TAG, e.Text);
            writer.WriteElementString(Part.DragonNS.WavePath.TAG, e.WavePath);
			writer.WriteEndElement();
		}
	}
}
