using System;
using System.IO;
using System.Windows.Forms;
using InputLog.Core.IO.Basic;
using InputLog.Core.IO.Convert;
using InputLog.Core.Util;
using InputLog.Core.IO.Convert.FileFormatValidator;
using System.Collections.Generic;

namespace GUI.Tabs.Preprocess.Conversion
{
    public partial class Convert : UserControl
    {
        #region Fields

        /// <summary>
        /// The logging format selected.
        /// </summary>
        public string ThisLogFormat { get; private set; }
    
        /// <summary>
        ///     The GUI of which this tab is part of.
        /// </summary>
        public Gui GUI;

        /// <summary>
        /// Validates a Translog file against its xsd and the xml extension.
        /// </summary>
        private readonly TranslogFileValidator TlValidator;

        private string SourceFile { get; set; }
        private string DestinationFile { get; set; }

        #endregion

        /// <summary>
        ///     Constructs the Convert tab.
        /// </summary>
        public Convert()
        {
            InitializeComponent();

            object[] inputFormats =
                {
                    new KeyValuePair<string,string>("Inputlog (< 5.0) IDF (*.idf)", LogFormat.IDF),
                    new KeyValuePair<string,string>("Inputlog (< 5.0) XML (*.xml)", LogFormat.LEGACY_XML),
                    new KeyValuePair<string,string>("Current Inputlog IDFX (*.idfx)", LogFormat.XML),
                    new KeyValuePair<string,string>("Translog (*.xml)", LogFormat.TRANSLOG_XML)
                };
            ConvertInputFormatList.Items.AddRange(inputFormats);
            ConvertInputFormatList.SelectedIndex = 3;

            object[] outputFormats =
                {
                    new KeyValuePair<string,string>("Current Inputlog IDFX (*.idfx)", LogFormat.XML),
                    new KeyValuePair<string,string>("Plaintext (*.txt)", LogFormat.TXT)
                };
            ConvertOutputFormatList.Items.AddRange(outputFormats);
            ConvertOutputFormatList.SelectedIndex = 0;

            TlValidator = new TranslogFileValidator();

            //TODO fix this ...
            /*this.Tabs.SelectedIndexChanged += delegate(object sender, EventArgs e) {
                this.ConvertProgressBar.Value = 0;
                this.ConvertProgressMessage.ResetText();
            };
             * */
        }

        public void SetSrcFile(string src)
        {
            ConvertDestFileTextField.Text = "";
            ConvertSrcFileTextField.Text = src;
            ConvertSrcFileTextField.Select(ConvertSrcFileTextField.Text.Length, 0);
        }

        /// <summary>
        ///     Click listener for the source file browse button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ConvertSrcFileButtonClick(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = ConvertSrcFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ConvertDestFileTextField.Text = "";
                    ConvertSrcFileTextField.Text = ConvertSrcFileDialog.FileName;
                    ConvertSrcFileTextField.Select(ConvertSrcFileTextField.Text.Length, 0);
                    Activate();
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        ///     Click listener for the destination file browse button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ConvertDestFileButtonClick(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = ConvertDestFileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ConvertDestFileTextField.Text = ConvertDestFileDialog.FileName;
                    ConvertDestFileTextField.Select(ConvertDestFileTextField.Text.Length, 0);
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        ///     Actually convert the data.
        /// </summary>
        public void DoConversion()
        {
            //Activate();
            try
            {
                // Check whether input and outputformats have been selected.
                if (ConvertInputFormatList.SelectedItem == null)
                {
                    MessageBox.Show("Please select the inputformat for the source file.",
                                    "Choose an inputformat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ConvertOutputFormatList.SelectedItem == null)
                {
                    MessageBox.Show("Please select the outputformat for the destination file.",
                                    "Choose an outputformat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                KeyValuePair<string, string> inputFormat = (KeyValuePair<string, string>) ConvertInputFormatList.SelectedItem;
                ThisLogFormat = inputFormat.Value;

                // Validate a Translog file against its XML schema (Translog.xsd).
                if (ThisLogFormat == LogFormat.TRANSLOG_XML)
                {
                    if (!TlValidator.Validate(SourceFile))
                    {      
                        //ConvertSrcFileTextField.Text = "";
                        //ConvertDestFileTextField.Text = "";
                        //ConvertSrcFileTextField.Focus();
                        MessageBox.Show(TlValidator.Remark(), "Invalid Translog file", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);                    
                        return;
                    }
                }
                KeyValuePair<string, string> outputFormat = (KeyValuePair<string, string>)ConvertOutputFormatList.SelectedItem;

                var logFormatConversion = new LogFileConvertor(ThisLogFormat, SourceFile, outputFormat.Value,
                                                               DestinationFile);

                logFormatConversion.DoConversion();

                // TODO What was up with the thread??
                //// Thread that does the actual converting by calling doConversion() on LogFileConvertor
                //System.Threading.Thread thread = new System.Threading.Thread
                //(delegate() {
                //    // use Invoke() in seperate delegate to avoid threading problems with user controls
                //    logFormatConversion.ProgressListeners += delegate(object send, ProgressEventArgs eventArgs) {
                //        this.Invoke(stepHandler, new object[] { send, eventArgs });
                //    };

                //    try {
                //        logFormatConversion.doConversion();
                //    } catch (ConversionException excep) {
                //        ExceptionMessageBox.Show(excep);
                //    }
                //});
                //thread.Start();
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        ///     Resets and Activates the Convertpanel.
        /// </summary>
        /// <param name="srcFileText">Path that is entered in the source file field.</param>
        /// <param name="destFileText">Path that is entered in the destination file field.</param>
        private void Activate(string srcFileText = "", string destFileText = "")
        {
            // reset progressbar
            //     ConvertProgressMessage.Text = "";
            //     ConvertProgressBar.Value = 0;

            // set source and destination fields/formats
            if (srcFileText != "")
            {
                ConvertSrcFileTextField.Text = srcFileText;
                ConvertSrcFileTextField.Select(ConvertSrcFileTextField.Text.Length, 0);
            }
            else
            {
                srcFileText = ConvertSrcFileTextField.Text;
            }
            switch (Path.GetExtension(srcFileText))
            {
                case ".idf":
                    {
                        ConvertInputFormatList.SelectedIndex = 0; // TXT
                        break;
                    }
                case ".idfx":
                    {
                        ConvertInputFormatList.SelectedIndex = 2; // TXT
                        break;
                    }
                case ".xml":
                    {
                        break;
                    }
                default:
                    ConvertInputFormatList.SelectedIndex = 1; // TXT
                    break;
            }

            if (destFileText == "")
            {
                destFileText = ConvertDestFileTextField.Text;
            }
            if (destFileText == string.Empty)
            {
                if (Path.GetExtension(srcFileText) == ".idfx")
                {
                    ConvertDestFileTextField.Text = Path.Combine(Path.GetDirectoryName(srcFileText),
                                                                 Path.GetFileNameWithoutExtension(srcFileText) + ".txt");
                    ConvertDestFileTextField.Select(ConvertDestFileTextField.Text.Length, 0);
                    ConvertOutputFormatList.SelectedIndex = 1; // TXT
                }
                else
                {
                    ConvertDestFileTextField.Text = Path.Combine(Path.GetDirectoryName(srcFileText),
                                                                 Path.GetFileNameWithoutExtension(srcFileText) + ".idfx");
                    ConvertDestFileTextField.Select(ConvertDestFileTextField.Text.Length, 0);
                    ConvertOutputFormatList.SelectedIndex = 0; // IDFX
                }
            }
            else
            {
                ConvertDestFileTextField.Text = destFileText;
                ConvertDestFileTextField.Select(ConvertDestFileTextField.Text.Length, 0);
                switch (Path.GetExtension(destFileText))
                {
                    case ".txt":
                        {
                            ConvertOutputFormatList.SelectedIndex = 1; // TXT
                            break;
                        }
                    case ".idfx":
                        {
                            break;
                        }
                    default:
                        {
                            ConvertOutputFormatList.SelectedIndex = 0; // IDFX
                            break;
                        }
                }
                ConvertOutputFormatList.SelectedIndex = 0; // IDFX
            }

            SourceFile = ConvertSrcFileTextField.Text;
            DestinationFile = ConvertDestFileTextField.Text;
            // switch active tab
            //GUI.Tabs.SelectedIndex = 3; // modify this when the order of the tabs change !
        }

        /// <summary>
        ///     Drag event handler.
        ///     Allows the user to drag files to the Convert tab (checks whether file is of type idf, xml or idfx).
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event Arguments.</param>
        private void ConvertDragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                {
                    var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length >= 1)
                    {
                        // FUTURE support batch processing
                        string extension = Path.GetExtension(files[0]);
                        switch (extension)
                        {
                                // IDFX file, switch to analyze tab
                            case ".xml":
                            case ".idf":
                            case ".idfx":
                                {
                                    e.Effect = DragDropEffects.All;
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }

        /// <summary>
        ///     Drop event handler.
        ///     Activates the convert tab with the dragged file as source file.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event Arguments.</param>
        private void ConvertDragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
                Activate(files[0]);
            }
            catch (Exception exc)
            {
                MessageLogger.CatchException(this, exc, Severity.ERROR);
            }
        }
    }
}