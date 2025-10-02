using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GUI.Tabs.Analyze
{
    /// <summary>
    ///  User dialog concerning the directory to save special analysis results and configurations.
    /// </summary>
    public partial class AnalyzeDialog : Form
    {
        /// <summary>
        /// Boolean confirming to save the analysis configurations. 
        /// </summary>
        private bool YesClicked { get; set; }

        /// <summary>
        ///  @"^(?!^(PRN|AUX|CLOCK\$|NUL|CON|COM\d|LPT\d|\..*)(\..+)?$)[^\x00-\x1f\\?*:\"";|/]+$";
        /// Regex pattern with invalid path/file name characters.
        /// </summary>
        private const string VALID_FILE_NAME_CHARS = @"\w{3,50}?";
        /// <summary>
        /// ErrorProvider emits warnings during the validation of the user input.
        /// </summary>
        private readonly ErrorProvider EventErrorProvider = new ErrorProvider();
        /// <summary>
        /// The folder name for the analysis results provided by the user.
        /// </summary>
        public string AnalysisFolder { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AnalyzeDialog()
        {
            InitializeComponent();
            YesClicked = false;
            okBtn.DialogResult = DialogResult.OK; 
        }

        /// <summary>
        /// Finalizes the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkBtnClick(object sender, EventArgs e)
        {
            AnalysisFolder = analysisDestinationTbx.Text;
        }

        /// <summary>
        /// User want to save the analysis configurations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YesBtnClick(object sender, EventArgs e)
        {
            YesClicked = true;
        }

        /// <summary>
        /// Validating the user input.
        /// Emits a flashing error icon next to the TextBox with the invalid input.
        /// </summary>
        /// <param name="sender">Input as text</param>
        /// <param name="e">Data for a cancelable event </param>
        private void ValidateInput(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string errorMsg;
            if (!IsValidInput(analysisDestinationTbx.Text, out errorMsg))
            {
                e.Cancel = true;
                analysisDestinationTbx.Text = "";
                analysisDestinationTbx.Select(0, analysisDestinationTbx.Text.Length);
                EventErrorProvider.SetError(analysisDestinationTbx, errorMsg);
            }
        }

        /// <summary>
        /// Removes error messages (if any) when input is valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputValidated(object sender, EventArgs e)
        {
            EventErrorProvider.SetError(analysisDestinationTbx, "");
        }

        /// <summary>
        /// Validate folder name.
        /// </summary>
        private static bool IsValidInput(string dirPath, out string errorMessage)
        {
            if (Regex.IsMatch(dirPath, VALID_FILE_NAME_CHARS, RegexOptions.CultureInvariant))
            {
                errorMessage = "";
                return true;
            }
            errorMessage = string.Format("The path name '" + dirPath + "'\ncontains invalid characters.");
            return false;
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtnClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}