using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InputLog.Core.Events;
using System.Xml;
using InputLog.Core.Events.EyeTracking;
using InputLog.Core.Events.EyeTracking.SubParts;
using InputLog.Core.Events.DragonNS;

namespace InputLog.Core.IO.Xml.Input
{
	public class XmlDragonNSReader
	{
		/// <summary>
		/// Reads an EventPart from a XmlElement.
		/// </summary>
		/// <param name="xmlElement">XmlElement to read the EventPart from.</param>
		/// <param name="eventType">In this case eventType would always be "eyetrack"</param>
		/// <returns>The EventPart that was read from the XmlElement.</returns>
		public static IEventPart ReadDragonns(XmlElement xmlElement)
		{
			DragonNSPart eventPart = new DragonNSPart();
            eventPart.ID = int.Parse(xmlElement[XmlElements.Log.Events.Event.Part.DragonNS.ID.TAG].InnerText);
            eventPart.TotalStartTime = ulong.Parse(xmlElement[XmlElements.Log.Events.Event.Part.DragonNS.TotalStartTime.TAG].InnerText);
            eventPart.TotalEndTime = ulong.Parse(xmlElement[XmlElements.Log.Events.Event.Part.DragonNS.TotalEndTime.TAG].InnerText);
            eventPart.StartTime = ulong.Parse(xmlElement[XmlElements.Log.Events.Event.Part.DragonNS.StartTime.TAG].InnerText);
            eventPart.EndTime = ulong.Parse(xmlElement[XmlElements.Log.Events.Event.Part.DragonNS.EndTime.TAG].InnerText);
            eventPart.Guid = xmlElement[XmlElements.Log.Events.Event.Part.DragonNS.Guid.TAG].InnerText;
            eventPart.Text = xmlElement[XmlElements.Log.Events.Event.Part.DragonNS.Text.TAG].InnerText;
            eventPart.WavePath = xmlElement[XmlElements.Log.Events.Event.Part.DragonNS.WavePath.TAG].InnerText;
			return eventPart;
		}

	}
}
