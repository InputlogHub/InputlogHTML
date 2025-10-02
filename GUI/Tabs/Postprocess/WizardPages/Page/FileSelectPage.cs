using System;
using System.Collections.Generic;
using GUI.Wizard;
using GUI.Tabs.Postprocess.WizardPages.Form;

namespace GUI.Tabs.Postprocess.WizardPages.Page
{
    internal class FileSelectPage : WizardPage
    {
        #region Fields

        /// <summary>
        /// Data key for the selected files.
        /// </summary>
        public const string FILES = "selected_files";

        /// <summary>
        /// Data key for the path to the selected folder.
        /// </summary>
        public const string PATH = "path";

        #endregion

        public FileSelectPage()
        {
            Control = new FileWizardPageForm();
            Next = new MergeAnalysisProcessPage();
            Next.Previous = this;
        }

        protected override Dictionary<string, Object> GetData()
        {
            var controlPage = (FileWizardPageForm) Control;
            if (Data.ContainsKey(PATH))
            {
                Data[PATH] = controlPage.GetPath();
            }
            else
            {
                Data.Add(PATH, controlPage.GetPath());
            }

            if (Data.ContainsKey(FILES))
            {
                Data[FILES] = controlPage.GetSelectedFiles();
            }
            else
            {
                Data.Add(FILES, controlPage.GetSelectedFiles());
            }

            // Merge with data from previous pages.
            return base.GetData();
        }

        /// <summary>
        /// Checks whether this page is completely filled in or not, and whether
        /// we can continue the next page.
        /// </summary>
        /// <returns>True if files have been selected, false if not a single file has been selected</returns>
        public override bool Validate()
        {
            var controlPage = (FileWizardPageForm) Control;

            List<string> fileNames = controlPage.GetSelectedFiles();
            // bool isNotEmpty = controlPage.GetSelectedFiles().Count > 0;
            if (fileNames.Count == 0)
            {
                LastError = "You must at least select one file.";
                return false;
            }

            // Check if the selected files can be merged according to the selected direction (vertical or horizontal)
            // Vertical merge MergeDirection 'true': General Analysis
            LastError = "";
            foreach (var name in fileNames)
            {
                bool abbrFound = false;
                foreach (string abbr in ((FileWizardPageForm)Control).Abbreviations[MergeAnalysis_Intro_Form.MergeDirection])
                {
                    if (name.Contains(abbr))
                    {
                        abbrFound = true;
                    }
                        
                }
                if (!abbrFound)
                {
                    LastError = MergeAnalysis_Intro_Form.MergeDirection ?
                        "Vertical merge is suited for General Analysis (GA), General Eyetrack Analysis (GEA), " +
                        "Revision (RM) or Linguistic Analysis (LG) only." :
                        "Horizontal merge is suited for Source Analysis (SO), Summary Analysis (SU), " +
                        "Fluency Analysis (FLUA), Revision (RM), Pause Analysis (PA) or Copytask  Analysis (CT) only.";
                    return false;
                }
            }
            return true;
        }

        internal void Reset()
        {
            ((FileWizardPageForm)Control).Reset();
        }
    }
}