using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using InputLog.Core.Analyses;
using InputLog.Core.Analyses.LinguisticAnalysis;
using InputLog.Core.Util;

namespace GUI.Tabs.Analyze.AnalysesControls.Linguistic
{
    public class LinguisticAnalyzer : AbstractAnalyzer
    {
        /// <summary>
        /// Name of the analysis
        /// </summary>
        public const string NAME = "Linguistic";

        /// <summary>
        /// Abbreviation for the analysis
        /// </summary>
        public const string ABBR = "LG";

        private readonly ulong PauseThreshold;

        public LinguisticAnalyzer()
        {
            PauseThreshold = 2000;
        }

        public override AnalysisDescription GetDescription(string workDir)
        {
            if (!SessionId.HasLanguage())
            {
                string lang = LanguageDialog();
                SessionId.SetLanguage(lang);
            }
            // Determine the startoffset if it is set.
           //  String tmp = SessionId.GetCreationDateString();
            var orgDocPath = OrgDocPath;
            if (!String.Empty.Equals(orgDocPath))
            {
                var newDocPath = Path.GetDirectoryName(orgDocPath) + "\\Wordlog.docx";

                if (File.Exists(orgDocPath) && File.Exists(newDocPath))
                {
                    var path1 = Path.Combine(workDir, "Wordlog_org.docx");
                    var path2 = Path.Combine(workDir, "Wordlog.docx");
                    path1 = PathSanitizer.Uniquify(path1);
                    path2 = PathSanitizer.Uniquify(path2);
                    var out1 = new FileStream(path1, FileMode.Create);
                    var in1 = new FileStream(orgDocPath, FileMode.Open);
                    in1.CopyTo(out1);
                    in1.Close();
                    out1.Close();
                    var out2 = new FileStream(path2, FileMode.Create);
                    var in2 = new FileStream(newDocPath, FileMode.Open);
                    in2.CopyTo(out2);
                    in2.Close();
                    out2.Close();
                    orgDocPath = "Work\\" + Path.GetFileName(path1);
                }
            }
            var descr = new LinguisticAnalysisDescription(Events, SessionId, ABBR, orgDocPath, PauseThreshold)
                        {
                            SessionID = SessionId
                        };
            return descr;
        }

        private static String LanguageDialog()
        {
            var form = new Form();
            var label = new Label();
            var langs = new ComboBox();
            var buttonOk = new Button();

            form.Text = "Please select your text language";
            label.Text = "Language";

            buttonOk.Text = "OK";
            buttonOk.DialogResult = DialogResult.OK;

            langs.Items.AddRange(Record.Record.LANGUAGES);
            langs.SelectedItem = langs.Items[Record.Record.DEFAULT_LANG];
            langs.DropDownStyle = ComboBoxStyle.DropDownList;
            langs.FlatStyle = FlatStyle.Flat;

            label.SetBounds(30, 20, 60, 20);
            langs.SetBounds(120, 15, 75, 20);
            buttonOk.SetBounds(120, 50, 75, 27);

            label.AutoSize = true;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(300, 85);
            form.Controls.AddRange(new Control[] { label, langs, buttonOk });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;

            form.ShowDialog();
            return langs.SelectedItem.ToString();
        }
    }
}
