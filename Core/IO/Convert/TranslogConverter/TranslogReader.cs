using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.XPath;
using InputLog.Core.Events;
using InputLog.Core.Events.WordLog;

namespace InputLog.Core.IO.Convert.TranslogConverter
{
    /// <summary>
    /// Reading Translog events and converting into Inputlog events.
    /// </summary>
    public class TranslogReader 
    {
        #region Fields

        /// <summary>
        /// XmlDocument used to parse and process the XML.
        /// </summary>
        private readonly XPathDocument TlDoc;

        /// <summary>
        /// Navigating and editing XML data nodes.
        /// </summary>
        private readonly XPathNavigator Nav;

        /// <summary>
        /// Stream containing the log.
        /// </summary>
        private readonly Stream Stream;

        /// <summary>
        /// Conversion factory for the different Translog events.
        /// </summary>
        private readonly TranslogFactory Factory;

        /// <summary>
        /// A list of converted Translog objects.
        /// </summary>
        List<Event> ConvertedTranslogList { get; set; }

        /// <summary>
        /// An internal utility class holding document length, cursor position,
        /// and event id for the event list being converted.
        /// </summary>
        private DocParam Param { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="objectStream">The Translog events</param>
        public TranslogReader(FileStream objectStream)
        {
            Stream = objectStream;
            TlDoc = new XPathDocument(Stream);
            Nav = TlDoc.CreateNavigator();
            Factory = new TranslogFactory();
            ConvertedTranslogList = new List<Event>();
        }

        /// <summary>
        /// Retrieves the Translog project description from the stream.
        /// </summary>
        /// <returns>Session information.</returns>
        public SessionIdentification ReadHeader()
        {
            var sessionIdentification = new SessionIdentification();
            sessionIdentification.SetLogVersion(Application.ProductVersion);
            var version = Nav.SelectSingleNode("/LogFile/VersionString");
            if (null != version)
            {
                sessionIdentification.SetConversion("Translog " + version);
            }
            var creation = Nav.SelectSingleNode("/LogFile/startTime");
            if (null != creation)
            {
                var creationDate = creation.ToString();
                var sb = new StringBuilder(creationDate.Length);
                foreach (char c in creationDate)
                {
                    if (c == '.')
                    {
                        break;
                    }
                    if(c =='-')
                    {
                        sb.Append('/');
                        continue;
                    }
                    sb.Append(c == 'T' ? ' ' : c);
                }

                sessionIdentification.SetCreationDate(sb.ToString());
            }
            var logName = Nav.SelectSingleNode("/LogFile/Project/Description");    
            if( null!= logName)
            {
            sessionIdentification.SetParticipant(logName.ToString());
            }

            return sessionIdentification;
        }

        /// <summary>
        /// Reads the objects from the stream.
        /// </summary>
        /// <returns>A list of Translog objects that were read from the stream 
        /// and converted into Inputlog Events.</returns>
        public IEnumerable<Event> ReadObjects()
        {
            // We first extract text written before the logging started. 
            // It becomes the first Inputlog event.
            var introNode = Nav.SelectSingleNode("/LogFile/Project/Interface/Standard/Settings/TargetText");
            const string pattern1 = @"(\\?fs\d+)";
            const string pattern2 = @"(\\par)";
            Param = new DocParam();
            if (introNode != null && introNode.InnerXml.Length != 0)
            {
                string intro = introNode.ToString();
                int start = intro.LastIndexOf("f0\\") + 3;
                int end = intro.LastIndexOf("\\par");
                intro = intro.Substring(start, end-start);
                intro = Regex.Replace(intro, pattern1, "");
                intro = Regex.Replace(intro, pattern2, "").Trim();
                if(intro != string.Empty) AddIntroEvent(intro);
            }

            foreach (XPathNavigator nodeEvent in Nav.Select("/LogFile/Events"))
            {
                ConvertedTranslogList.AddRange(Factory.GetObject(nodeEvent, Param));
            }
            return ConvertedTranslogList;
        }

        /// <summary>
        /// A not-logged introductory text in Translog is saved as
        /// an insert event before starting the regular conversion.
        /// </summary>
        /// <param name="intro">Text to add upfront as insert.</param>
        private void AddIntroEvent(string intro)
        {
            var thisEvent = new Event();
            thisEvent.Properties["type"] = "insert";
            thisEvent.Properties["id"] = (Param.Id++).ToString();
            thisEvent.Parts.Add(new Insert(0, "", intro));
            ConvertedTranslogList.Add(thisEvent);

            Param.DocLength = intro.Length;
            Param.Position = intro.Length;
        }

        /// <summary>
        /// Document parameters.
        /// </summary>
        public class DocParam
        {
             public int DocLength { get; set; }
             public int Position { get; set; }
             public int Id { get; set; }
        }
    }
}
