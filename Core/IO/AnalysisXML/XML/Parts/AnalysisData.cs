using System.Collections.Generic;
using InputLog.Core.Util;

namespace InputLog.Core.IO.AnalysisXML.XML.Parts
{
    public class AnalysisData
    {
        #region Fields

        /// <summary>
        /// Title of the data if any.
        /// </summary>
        public string Title;

        /// <summary>
        /// Properties of the object.
        /// </summary>
        public Dictionary<string, string> Properties { get; private set; }

        /// <summary>
        /// Attributes of the object.
        /// </summary>
        public Dictionary<string, string> Attributes { get; private set; }

        /// <summary>
        /// Child entitites are listed here.
        /// </summary>
        public List<AnalysisData> Children { get; private set; }

        #endregion

        /// <summary>
        /// Construct the AnalysisData object.
        /// </summary>
        protected AnalysisData()
        {
            Properties = new Dictionary<string, string>();
            Attributes = new Dictionary<string, string>();
            Children = new List<AnalysisData>();
        }

        public override bool Equals(object obj)
        {
            if (obj is AnalysisData)
            {
                AnalysisData ad = (AnalysisData)obj;
                if (Children.Count != ad.Children.Count) return false;
                if (!Properties.SameContents(ad.Properties)) return false;
                if (!Attributes.SameContents(ad.Attributes)) return false;

                for (int i = 0; i < Children.Count; i++)
                {
                    if (!Children[i].Equals(ad.Children[i])) return false;
                }
                return true;
            }
            return base.Equals(obj);
        }
    }
}