using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using InputLog.Core.IO;
using log4net;
using System.Globalization;
using InputLog.Core.Util;

namespace InputLog.Core.Analyses.Focus
{
    /// <summary>
    /// Analysis XMLWriter for the Focus Analysis.
    /// </summary>
    public class FocusAnalysisXMLWriter : AbstractAnalysisXMLWriter
    {
        #region Constants
        //private const string VISUAL_REPRESENTATION_SRC = "FocusAnalysis\\visualrepresentation.png";
        //private const string VISUAL_REPRESENTATION_PATH_TAG = "visualRepresentationPath";

        // Tags and tag-values used in the analysis XML document.
        private const string STYLESHEET_HREF = "source_analysis.xsl";

        private const string MODULE_TAG = "module";
        private const string BLOCK_TAG = "block";
        private const string ELEMENT_TAG = "element";

        // statistics
        private const string WINDOWS_TAG = "windows";
        private const string WINDOW_TAG = "window";
        private const string SUMMARY_TAG = "summary";
        private const string TITLE_TAG = "title";
        private const string TOTAL_TIME_TAG = "totalTime";
        private const string TOTAL_TIME_RELATIVE_TAG = "totalTimeRelative";
        private const string RELATIVE_TIME_TOTAL = "relativeTimeTotal";
        private const string TOTAL_KEYPRESSES = "totalKeypresses";
        private const string RELATIVE_KEYPRESS_TOTAL= "relativeKeyPressTotal";

        // transitions
        private const string TRANSITIONS_TAG = "transitions";
        private const string TRANSITION_TAG = "transition";
        private const string TRANSITION_FROM_TAG = "transitionFrom ";
        private const string TRANSITION_TO_TAG = "transitionTo ";
        private const string TRANSITION_COUNT_TAG = "transitionCount ";
        private const string TOTAL_TRANSITIONS_TAG = "totalTransitions";

        // Intervals
        private const string SUMMARY_PER_INTERVAL = "Windows and Transitions per Interval";
        private const string INTERVAL = "Interval ";
        private const string INTERVAL_START = "Start Time";
        #endregion

        #region Fields
        /// <summary>
        /// Log4Net MessageLogger.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Count of the total transition time
        /// </summary>
        private ulong TotalTime;
        // Formatting a numerical value.
        private readonly NumberFormatInfo Nfi = new CultureInfo("en-US", false).NumberFormat;

        private FocusAnalysisSummary focusAnalysisSummary;
        #endregion

        #region Pajek Fields
        // Colors for the visual representation in Pajek
        private enum Colors
        {
            GreenYellow = 1, Fuchsia = 2, JungleGreen = 3, Yellow = 4, Lavender = 5, SeaGreen = 6,
            Goldenrod = 7, Thistle = 8, Green = 9, Dandelion = 10, Orchid = 11, ForestGreen = 12, Apricot = 13,
            DarkOrchid = 14, PineGreen = 15, Peach = 16, Purple = 17, LimeGreen = 18, Melon = 19, Plum = 20,
            YellowGreen = 21, YellowOrange = 22, Violet = 23, SpringGreen = 24, Orange = 25, RoyalPurple = 26,
            OliveGreen = 27, BurntOrange = 28, BlueViolet = 29, RawSienna = 30, Bittersweet = 31, Periwinkle = 32,
            Sepia = 33, RedOrange = 34, CadetBlue = 35, Brown = 36, Mahogany = 37, CornflowerBlue = 38, Tan = 39,
            Maroon = 40, MidnightBlue = 41, Gray = 42, BrickRed = 43, NavyBlue = 44, LightYellow = 45, Red = 46,
            RoyalBlue = 47, LightCyan = 48, OrangeRed = 49, Blue = 50, LightMagenta = 51, RubineRed = 52,
            Cerulean = 53, LightPurple = 54, WildStrawberry = 55, Cyan = 56, LightGreen = 57, Salmon = 58,
            ProcessBlue = 59, LightOrange = 60, CarnationPink = 61, SkyBlue = 62, Canary = 63, Magenta = 64,
            Turquoise = 65, LFadedGreen = 66, VioletRed = 67, TealBlue = 68, Pink = 69, Rhodamine = 70,
            Aquamarine = 71, LSkyBlue = 72, Mulberry = 73, BlueGreen = 74, RedViolet = 75, Emerald = 76,
            Gray05 = 77, Gray15 = 78, Gray25 = 79, Gray35 = 80, Black = 81, White = 82
        }
        private static readonly string[] ColorNames = Enum.GetNames(typeof(Colors));
        /// <summary>
        /// Dictionaries with indexes of the focus events in Pajek
        /// </summary>
        private IDictionary<string, Int32> NodeDict;
        private IDictionary<Int32, double> NodeValueDict;
        /// <summary>
        /// Path to the Pajek Vizualization input file;
        /// </summary>
        private readonly string PajekPath;
        /// <summary>
        /// The Pajek string builder
        /// </summary>
        private readonly StringBuilder PajekString = new StringBuilder();
        /// <summary>
        /// True if the checkbox 'Add a Pajek File' is checked in the GUI.
        /// </summary>
        public static bool WritePajekFile { private get; set; } 
        #endregion
        
        /// <summary>
        /// Constructs a FocusAnalysisXMLWriter.
        /// </summary>
        /// <param name="destinationFilePath">Path where the analysisDocument should be saved.</param>
        public FocusAnalysisXMLWriter(string destinationFilePath)
            : base(destinationFilePath)
        {
            PajekPath = Path.ChangeExtension(destinationFilePath, ".net");
        }

        /// <summary>
        /// Writes out the analysis document using a given FocusAnalysisSummary.
        /// </summary>
        /// <param name="sessionIdentification">SessionIdentification of the logging session on which
        ///  the Focus Analysis was performed.</param>
        /// <param name="extraInfo"></param>
        /// <param name="summary">Summary of the analysis.</param>
        public override void WriteDocument(SessionIdentification sessionIdentification,
            IDictionary<string, IDictionary<string, object>> extraInfo, IAnalysisSummary summary)
        {
            if (!(summary is FocusAnalysisSummary))
                throw new AnalysisWriterException("Given summary is not of type FocusAnalysisSummary");

            // Max. number of decimal digits to show.
            Nfi.NumberDecimalDigits = 3;
            //  Displays a blank as the thousand separator instead of the default comma.
            Nfi.NumberGroupSeparator = " ";

            WriteHeader(STYLESHEET_HREF);
            XMLWriter.WriteStartElement(SESSION_TAG);
            WriteSessionMetaData(sessionIdentification);
            WriteSessionIdentification(sessionIdentification);
            WriteExtraInfo(extraInfo);
           focusAnalysisSummary = (FocusAnalysisSummary)summary;
            WriteStats(focusAnalysisSummary);
            CopyStyle(new[] { STYLESHEET_HREF, COMMON_XSL, COMMON_CSS }, new[] { INPUTLOG_LOGO }, new[] { JQUERY_SCRIPT });

            // Writes a Pajek visual representation to disk if the checkbox in the Focus Analysis GUI is selected.
            if (WritePajekFile)
            {
                ProcessPajek(focusAnalysisSummary);
                WriteVisualRepresentation(PajekString.ToString());
            }
        }

        private void WriteStats(FocusAnalysisSummary summary)
        {
            var windowParams = new Dictionary<string, string>();
            IDictionary<string, FocusAnalysisSummary.WindowStatistics> windowStats = summary.WindowStats;
            WriteModule("Window Statistics", windowParams, () => WriteWindowStats(windowStats));
         
            IDictionary<string, IDictionary<string, int>> transitions = summary.WindowTransitionCounts;
            var transitionParams = new Dictionary<string, string>
            { 
                { TOTAL_TRANSITIONS_TAG, summary.TotalWindowTransitions.ToString() },
            };
            WriteModule("Window Transition Statistics", transitionParams, () => WriteTransitionStats(transitions));

            var intervalParams = new Dictionary<string, string>();

            // Write Interval Start Times
            WriteModule("Intervals", () =>
            {
                WriteModuleBlock(INTERVAL_START, () =>
                {
                    foreach (var stats in summary.IntervalInfo)
                    {
                        WriteModuleElement("Interval " + stats.Value.IntervalSegment, (stats.Value.IntervalStart/1000.0).ToString("F", Nfi));
                    }
                });
            });
            //WriteModule("Interval Statistics", intervalParams, () => WriteIntervalStats(summary.IntervalInfo));
            WriteIntervalStats(summary.IntervalInfo);
        }

        private void WriteTransitionStats(IDictionary<string, IDictionary<string, int>> transitions, int segment = -1, bool isInterval= false)
        {
            var s1 = TRANSITION_FROM_TAG;
            if (isInterval)
            {
                s1 = TRANSITION_FROM_TAG + segment;
            }
            WriteModuleBlock(s1, delegate
            {
                int c = 1;
                foreach (var transitionDictEntry in transitions)
                {
                    foreach (var windowCountEntry in transitionDictEntry.Value)
                    {
                        WriteModuleElement(GetTransitionID(transitionDictEntry.Key, windowCountEntry.Key), transitionDictEntry.Key);
                        c++;
                    }
                }
            }, true);

            var s2 = TRANSITION_TO_TAG;
            if (isInterval)
            {
                s2 = TRANSITION_TO_TAG + segment;
            }
            WriteModuleBlock(s2, delegate
            {
                int c = 1;
                foreach (var transitionDictEntry in transitions)
                {
                    foreach (var windowCountEntry in transitionDictEntry.Value)
                    {
                        WriteModuleElement(GetTransitionID(transitionDictEntry.Key, windowCountEntry.Key), windowCountEntry.Key);
                        c++;
                    }
                }
            }, true);

            var s3 = TRANSITION_COUNT_TAG;
            if (isInterval)
            {
                s3 = TRANSITION_COUNT_TAG + segment;
            }
            WriteModuleBlock(s3, delegate
            {
                int c = 1;
                foreach (var transitionDictEntry in transitions)
                {
                    foreach (var windowCountEntry in transitionDictEntry.Value)
                    {
                        WriteModuleElement(GetTransitionID(transitionDictEntry.Key, windowCountEntry.Key), windowCountEntry.Value.ToString());
                        c++;
                    }
                }
            });
        }

        /// <summary>
        /// Statistics on windows and their transitions per interval.
        /// </summary>
        /// <param name="summary">Summary of the analysis.</param>
        private void WriteIntervalStats(IDictionary<int, FocusAnalysisSummary.IntervalStats> summary)
        {
            // Write WindowStatistics
            foreach (var intervals in summary.Values)
            {
                var totals = intervals.IntervalTotals;
                var iStatsTitle = "Interval " + intervals.IntervalSegment + " - Window Statistics";

                WriteModule(iStatsTitle, () =>
                {
                    WriteWindowStats(intervals.IntervalFocusStats, intervals.IntervalSegment, true);
                    WriteModuleBlock("Totals " + intervals.IntervalSegment, delegate
                    {
                        WriteModuleElement(TOTAL_TIME_TAG, (totals.TotalTime/1000.0).ToString("F", Nfi));
                        WriteModuleElement(RELATIVE_TIME_TOTAL, totals.RelativeTimeTotal.ToString("F", Nfi));
                        WriteModuleElement(TOTAL_KEYPRESSES, totals.TotalKeyPresses.ToString());
                        WriteModuleElement(RELATIVE_KEYPRESS_TOTAL, totals.RelativeKeypressTotal.ToString("F", Nfi));
                    });

                });
            }

            // Write Window Transition Statistics
            foreach (var intervals in summary.Values)
            {
                var iWTransitionTitle = "Interval " + intervals.IntervalSegment + " - Window Transition Statistics";
                var transitionParams = new Dictionary<string, string>
                {
                    {TOTAL_TRANSITIONS_TAG, intervals.IntervalTotals.TotalWindowTransitions.ToString()},
                };
                WriteModule(iWTransitionTitle, transitionParams, () =>
                {
                    WriteTransitionStats(intervals.IntervalWindowTransitions, intervals.IntervalSegment, true);
                });
            }
        }

        private string GetTransitionID(string from, string to)
        {
            return "[" + from + "~" + to + "]";
        }

        private void WriteWindowStats(IDictionary<string, FocusAnalysisSummary.WindowStatistics> stats, int segment = -1, bool isInterval=false)
        {
            var s1 = "Total Time(s)";
            if (isInterval)
            { 
                s1 = "Total Time(s) " + segment;
            }
            WriteModuleBlock(s1, delegate
            {
                foreach (string w in stats.Keys)
                {
                    WriteModuleElement(w, (stats[w].TotalTime / 1000.0).ToString("F", Nfi));
                }
            });

            var s2 = "Total Time(relative)";
            if (isInterval)
            {
                s2 = "Total Time(relative) " + segment;
            }
            WriteModuleBlock(s2, delegate
            {
                foreach (string w in stats.Keys)
                {
                    WriteModuleElement(w, (stats[w].TotalTimeRelative).ToString("F", Nfi));
                }
            });

            var s3 = "Total Keystrokes ";
            if (isInterval)
            {
                s3 = "Total Keystrokes " + segment;
            }
            WriteModuleBlock(s3, delegate
            {
                foreach (string w in stats.Keys)
                {
                    WriteModuleElement(w, (stats[w].TotalKeyPresses).ToString());
                }
            });

            var s4 = "Total Keystrokes(relative) ";
            if (isInterval)
            {
                s4 = "Total Keystrokes(relative) " + segment;
            }
            WriteModuleBlock(s4, delegate
            {
                foreach (string w in stats.Keys)
                {
                    WriteModuleElement(w, (stats[w].TotalKeyPressesRelative).ToString("F", Nfi));
                }
            });

            // The interval module has its own 'Totals' block.
            if (isInterval) return;
            WriteModuleBlock("Totals", delegate
            {
                WriteModuleElement(TOTAL_TIME_TAG, (focusAnalysisSummary.TotalTime / 1000.0).ToString("F", Nfi));
                WriteModuleElement(RELATIVE_TIME_TOTAL, (focusAnalysisSummary.RelativeTimeTotal).ToString("F", Nfi));
                WriteModuleElement(TOTAL_KEYPRESSES, focusAnalysisSummary.TotalKeyPresses.ToString());
                WriteModuleElement(RELATIVE_KEYPRESS_TOTAL, focusAnalysisSummary.RelativeKeypressTotal.ToString("F", Nfi));
            });
        }

        /// <summary>
        /// Writes out the WindowStatistics of the focus analysis as Pajek graph
        /// </summary>
        /// <param name="summary">Focus Analysis summary that contains the dictionary 
        /// that maps window titles to WindowStatistics' to write to the analysis document.</param>
        private void ProcessPajek(FocusAnalysisSummary summary)
        {
            IDictionary<string, FocusAnalysisSummary.WindowStatistics> windowStats = summary.WindowStats;
            
            // Pajek dictionaries
            NodeDict = new Dictionary<string, Int32>();
            NodeValueDict = new Dictionary<Int32, double>();
            var nodeCountOffset = 0;
            
            foreach (var stats in windowStats)
            {
                // Pajek-file Part I - Nodes
                if (stats.Key.StartsWith("* Time"))
                {
                    nodeCountOffset = 1;
                    continue;
                }
                // Adding Pajek node data and total time
                TotalTime += stats.Value.TotalTime;
                NodeDict.Add(stats.Key, stats.Value.Index - nodeCountOffset);
                NodeValueDict.Add(stats.Value.Index - nodeCountOffset, stats.Value.TotalTimeRelative);
            }

            PajekString.AppendLine("*Arcs");

            foreach (var transitionDictEntry in summary.WindowTransitionCounts)
            {
                foreach (var windowCountEntry in transitionDictEntry.Value)
                {
                    try
                    {
                        // Part II - Arcs/Transitions
                        // Replaces the focus event labels with their indexes. 
                        int fromIdx, toIdx;
                        NodeDict.TryGetValue(transitionDictEntry.Key, out fromIdx);
                        NodeDict.TryGetValue(windowCountEntry.Key, out toIdx);
                        PajekString.AppendLine(fromIdx + " " + toIdx + " " + windowCountEntry.Value + " w 1");
                    }
                    catch (KeyNotFoundException)
                    {
                        //ignore
                    }
                }
            }
        }

        /// <summary>
        /// Writes out the visual description for the focus analysis as a Pajek file.
        /// </summary>
        /// <param name="pajekString">String containing the arc part of the Pajek file</param>
        private void WriteVisualRepresentation(string pajekString)
        {
            if(pajekString.IsNullOrEmpty()) return;
            var tmpBuilder = new StringBuilder();
            tmpBuilder.AppendLine("*Vertices " + NodeDict.Count);
            string ic;
            string bc;
            // Pajek-file Part I - Nodes
            // String to format the circles in a graph rendering of the focus analysis. 'x_fact' and 'y_fact' determine
            // the size of the circle based on the total time spent in a focus. 'ic' (inside color) is the color of
            // the circle, 'bc' (border color) is the color of the circle border. 'fos' is the size of the label font. 
            foreach (KeyValuePair<string, Int32> node in NodeDict)
            {
                // Transforming the raw node data into a ratio whereby no value will be smaller than 1.
                double value;
                NodeValueDict.TryGetValue(node.Value, out value);
                int nodeWeight = Convert.ToInt32(value * 100);
                if(nodeWeight < 5)
                {
                    nodeWeight = 5;
                }

                // If we have more sources than 82, their color becomes white.
                if (node.Value < 81)
                {
                  ic = ColorNames[node.Value];
                  bc = ColorNames[node.Value];
                }
                else
                {
                    ic = ColorNames[81];
                    bc = ColorNames[81];
                }
                var pajekFormat =
                    $" 0.0 0.0 0.5 ellipse x_fact {nodeWeight} y_fact {nodeWeight} ic {ic} bc {bc} lr 6 fos 10";
                tmpBuilder.AppendLine(node.Value + " " + '"' + node.Key + '"' + pajekFormat);
            }
            // Combining Part I and Part II
            tmpBuilder.Append(pajekString);
         
            // Defines a text writer.
            var pajekWriter = new StreamWriter(PajekPath);
            // Text writer writes the focus transition data to a Pajek text file.
            pajekWriter.Write(tmpBuilder.ToString());
            pajekWriter.Close();
        }

        /// <summary>
        /// Empty abstract method implementation
        /// </summary>
        /// <param name="summary"></param>
        /// <returns></returns>
        public override XmlDocument WriteMemory(IAnalysisSummary summary)
        {
            throw new NotImplementedException();
        }
    }
}