using System.IO;
using InputLog.Core.Merging.Analyses;

namespace InputLog.Core.IO.AnalysisXML.Input
{
    public class AnalysisReaderFactory
    {
        /// <summary>
        /// Create an appropriate XML reader depending on the analysis type. 
        /// </summary>
        /// <param name="type">Type of the analysis to be read.</param>
        /// <returns>An appropriate reader for the given analysis type. If the type is unknown
        /// this method returns null instead.</returns>
        public static BasicXMLReader Create(AnalysisType type)
        {
            switch (type)
            {
                case AnalysisType.GENERAL:
                    return new GeneralAnalysisReader();

                case AnalysisType.PAUSE:
                    return new PauseAnalysisReader();

                case AnalysisType.SUMMARY:
                    return new SummaryAnalysisReader();

                case AnalysisType.REVISION:
                    return new RevisionAnalysisReader();

                case AnalysisType.SNOTATION:
                    return new SNotationAnalysisReader();

                case AnalysisType.LINEAR:
                    return new LinearAnalysisReader();

                case AnalysisType.LINGUISTIC:
                    return new LinguisticAnalysisReader();

                case AnalysisType.WORD_PAUSES:
                    return new WordPausesAnalysisReader();

                case AnalysisType.GENERAL_EYETRACK:
                    return new GeneralEyetrackAnalysisReader();

                case AnalysisType.REPORTING:
                    return new ReportingXMLReader();

                default:
                    return new DefaultXMLReader();
            }
        }

        /// <summary>
        /// Determine the analysis type of the file based on the filePath. The filename 
        /// of analysis files use a naming convention where e.g. the file ends with:
        /// _GA.xml if it is a general analysis file, or e.g.
        /// _SA_PTxxxx.xml in case it is a summary analysis file. 
        /// This method determines the type based on the tokesn GA, SA or PA. If a file 
        /// has more than one of such tokens in its name, its type can not be unambiguously decided
        /// and it will trigger an error.
        /// </summary>
        /// <param name="filePath">complete path to the file.</param>
        /// <returns>The type of the analysis file.</returns>
        public static AnalysisType GetType(string filePath)
        {
            // Get filename and split name on all '_' and '.' 
            string fileName = filePath.Substring(filePath.LastIndexOf(Path.DirectorySeparatorChar));
            string[] fileNameParts = fileName.Split('_', '.', ' ');

            // Init on unknown.
            var type = AnalysisType.UNKNOWN;

            // If one of the parts matches a known token (e.g. GA) its type is set to that
            // type of analysis file. If another token match is found in the filename while
            // a type had already been assigned, the type can not be unambiguously decided.
            foreach (string part in fileNameParts)
            {
                switch (part)
                {
                    case "GA":
                        if (type != AnalysisType.UNKNOWN)
                        {
                            return AnalysisType.UNKNOWN;
                        }
                        type = AnalysisType.GENERAL;
                        break;

                    case "SU":
                        if (type != AnalysisType.UNKNOWN)
                        {
                            return AnalysisType.UNKNOWN;
                        }
                        type = AnalysisType.SUMMARY;
                        break;

                    case "PA":
                        if (type != AnalysisType.UNKNOWN)
                        {
                            return AnalysisType.UNKNOWN;
                        }
                        type = AnalysisType.PAUSE;
                        break;

                    case "FLUA":
                        if (type != AnalysisType.UNKNOWN)
                        {
                            return AnalysisType.UNKNOWN;
                        }
                        type = AnalysisType.FLUENCY;
                        break;

                    case "SO":
                        if (type != AnalysisType.UNKNOWN)
                        {
                            return AnalysisType.UNKNOWN;
                        }
                        type = AnalysisType.SOURCE;
                        break;

                    case "LG":
                        if (type != AnalysisType.UNKNOWN)
                        {
                            return AnalysisType.UNKNOWN;
                        }
                        type = AnalysisType.LINGUISTIC;
                        break;

                    case "GEA":
                        if (type != AnalysisType.UNKNOWN)
                        {
                            return AnalysisType.UNKNOWN;
                        }
                        type = AnalysisType.GENERAL_EYETRACK;
                        break;

                    case "RM":
                        if (type != AnalysisType.UNKNOWN)
                        {
                            return AnalysisType.UNKNOWN;
                        }
                        type = AnalysisType.REVISION;
                        break;
                    case "WP":
                        if (type != AnalysisType.UNKNOWN)
                        {
                            return AnalysisType.UNKNOWN;
                        }
                        type = AnalysisType.WORD_PAUSES;
                        break;
                    case "REP":
                        if (type != AnalysisType.UNKNOWN)
                        {
                            return AnalysisType.UNKNOWN;
                        }
                        type = AnalysisType.REPORTING;
                        break;
                }
            }
            return type;
        }
    }
}