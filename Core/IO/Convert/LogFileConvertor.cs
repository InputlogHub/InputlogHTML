using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using InputLog.Core.Events;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Basic.Input;
using InputLog.Core.IO.Basic.Output;
using InputLog.Core.IO.Convert.TranslogConverter;
using InputLog.Core.Util.Progress;

namespace InputLog.Core.IO.Convert
{
    /// <summary>
    /// Utility class for easy converting between logfiles in different formats.
    /// </summary>
    public class LogFileConvertor : ProgressTrackableAction
    {
        #region Fields

        /// <summary>
        /// The format from which this class will convert eventz.
        /// </summary>
        private string InputFormat { get; set; }

        /// <summary>
        /// The format to which this class will convert eventz.
        /// </summary>
        private string OutputFormat { get; set; }

        /// <summary>
        /// The path of source file.
        /// </summary>
        private string InputFilePath { get; set; }

        /// <summary>
        /// The path of the destination file.
        /// </summary>
        private string OutputFilePath { get; set; }

        /// <summary>
        /// Reference to the SystemLogger.
        /// </summary>
        //private readonly SystemLogger SysLog = SystemLogger.SysLog;

        #endregion

        /// <summary>
        /// Constructs a LogFileConvertor.
        /// The actual conversion is done by calling the doConversion() method.
        /// However, after constructing a FormatConversion, the FormatConversion has already estimated 
        /// the number of Steps (property) it will take to Convert the file.
        /// </summary>
        /// <param name="inputFormat">The format of the source-file.</param>
        /// <param name="inputFilePath">The path specifiying the source file.</param>
        /// <param name="outputFormat">The format of the destination-file.</param>
        /// <param name="outputFilePath">The path specifiying the destination file.</param>
        public LogFileConvertor(string inputFormat, string inputFilePath, string outputFormat,
                                string outputFilePath)
        {
            InputFormat = inputFormat;
            InputFilePath = inputFilePath;
            OutputFormat = outputFormat;
            OutputFilePath = outputFilePath;

            NumberOfSteps = 5;
            if (InputFormat == LogFormat.IDF)
            {
                NumberOfSteps++;
            }
        }

        /// <summary>
        /// Performs the actual conversion by reading the specified source file,
        /// converting the events to the new format and writing the result to the specified destinationfile.
        /// </summary>
        public void DoConversion()
        {
            // Make an exception for IDF
            // First convert IDF to Legacy XML using "Integrate.exe"
            // Then convert the generated Legacy XML to the new format
            var proc = new Process();

            switch (InputFormat)
            {
                case LogFormat.IDF:
                {
                    ReportProgress(this, new ProgressEventArgs("Converting IDF to InputLog 4.0 intermediate XML format."));

                    string legacyXMLFile = OutputFilePath + ".old";
                    // The legacy conversion.
                    LegacyProcess(proc, legacyXMLFile);

                    // Further processing is legacy XML
                    const string inputFormat = LogFormat.LEGACY_XML;
                    string srcFile = legacyXMLFile;

                    // Creates a reader
                    EventLogReader eventLogReader = EventLogFactory.CreateFileEventLogReader(srcFile, inputFormat);          
                    ReportProgress(this, new ProgressEventArgs("Reading session identification information from source file"));

                    // Reading session identification
                    SessionIdentification sessionIdentification = eventLogReader.ReadSessionIdentification();
                    ReportProgress(this, new ProgressEventArgs("Reading events from source file"));

                    // Adding some extra meta-info.
                    sessionIdentification.SetConversionFormats(InputFormat, OutputFormat);
                    sessionIdentification.SetRelativeCreationTime(0);

                    // Reading the events.
                    List<Event> events = eventLogReader.ReadEvents();
                    WriteEvents(sessionIdentification, events);
                }
                    break;
                case LogFormat.TRANSLOG_XML:
                {
                    ReportProgress(this, new ProgressEventArgs("Converting Translog to InputLog idfx format."));

                    // A Translog xml reader for the Translog objects.
                    var logReader = new TranslogReader(File.OpenRead(InputFilePath));
                    ReportProgress(this, new ProgressEventArgs("Reading the Translog data file"));

                    // Reading session identification
                    SessionIdentification sessionIdentification = logReader.ReadHeader();
                    sessionIdentification.SetRelativeCreationTime(0);

                    // Reading the Translog events and transforming them into Inputlog events.
                    List<Event> events = (List<Event>) logReader.ReadObjects();
                    WriteEvents(sessionIdentification, events);             
                }
                    break;
            }
        }

        /// <summary>
        /// The xml writer of a event file.
        /// </summary>
        /// <param name="sessionIdentification">Header info for the idx file.</param>
        /// <param name="events">The list of events to write.</param>
        private void WriteEvents(SessionIdentification sessionIdentification, List<Event> events)
        {
            EventLogWriter eventWriter = EventLogFactory.CreateFileEventLogWriter(OutputFilePath, OutputFormat);
            ReportProgress(this, new ProgressEventArgs("Writing events to a destination file"));
            eventWriter.WriteExistingFile(sessionIdentification, events);
            ReportProgress(this, new ProgressEventArgs("Done"));
        }

        /// <summary>
        /// The transformation of a legacy logging file into a xml file.
        /// </summary>
        /// <param name="proc">The process that executes the transformation</param>
        /// <param name="file">The file to tranform</param>
        private void LegacyProcess(Process proc, string file)
        {
            try
            {
                // Command: "Integrate.exe InputLog output.xml input.idf"
                proc.StartInfo.FileName = Properties.Settings.Default.Integrate_exe;
                proc.StartInfo.Arguments = "Inputlog \"" + file + "\" \"" + InputFilePath + "\"";
                proc.EnableRaisingEvents = false;

                // Redirecting output of process to catch error messages.
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;

                proc.Start();

                StreamReader stdOutputReader = proc.StandardOutput;
                StreamReader stdErrorReader = proc.StandardError;

                proc.WaitForExit();

                // If the exitcode > 0 an exception containing the output of Integrate.exe is thrown.
                if (proc.ExitCode > 0 && InputFormat == LogFormat.IDF)
                {
                    throw new ConversionException(
                        "Unable to convert IDF to Legacy XML (Integrate.exe returned exit code " + proc.ExitCode +
                        ")\n" +
                        "Standard output: " + stdOutputReader.ReadToEnd() + "\n" +
                        "Standard error: " + stdErrorReader.ReadToEnd() + "\n"
                        );
                }
            }
            finally
            {
                proc.Dispose();
            }
        }
    }
}