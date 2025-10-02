using System.Collections.Generic;
using System.IO;
using System.Linq;
using InputLog.Core.IO.Xml;

namespace GUI.Tabs.Preprocess
{
    /// <summary>
    /// Business logic for the Preprocess tab.
    /// </summary>
    public class PreprocessModel
    {
        private readonly List<string> ThisSourcePaths = new List<string>();

        public static readonly IDictionary<string, string> Keys = new Dictionary<string, string>
        {
            {"PageDown", "VK_NEXT"},
            {"Left Ctrl", "VK_LCONTROL"},
            {"Right Ctrl", "VK_RCONTROL"},
            {"=", "VK_OEM_PLUS"},
            {"Pause Break", "VK_PAUSE"}
        };

        public IList<string> SourcePaths
        {
            get { return ThisSourcePaths.AsReadOnly(); }
            set
            {
                ThisSourcePaths.Clear();
                if (value != null) ThisSourcePaths.AddRange(value.Where(File.Exists));
            }
        }

        public int SegmentIdfx(string splitKey, bool includeInitialPause)
        {
            var segmenter = new IdfxSegmenter(splitKey, includeInitialPause);
            return segmenter.Segment(SourcePaths.ToArray());
        }

        public void MergeIdfx(bool includeInitialPause)
        {
            var merger = new IdfxMerger(includeInitialPause);
            merger.Merge(SourcePaths.ToArray());
        }
    }
}