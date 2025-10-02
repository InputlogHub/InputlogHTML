using System.Diagnostics;
using System.Windows.Forms;

namespace GUI.Tabs.Postprocess.WizardPages.Form
{
    public partial class FileResultProcessPage : ProcessPage
    {
        public string FileDirectory;

        public FileResultProcessPage()
        {
            InitializeComponent();
        }

        private void OpenFolderLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FileDirectory))
            {
                Process.Start("explorer.exe", FileDirectory);
            }
        }
    }
}