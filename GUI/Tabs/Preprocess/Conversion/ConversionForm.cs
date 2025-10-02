using System;
using System.Windows.Forms;

namespace GUI.Tabs.Preprocess.Conversion
{
    public partial class ConversionForm : Form
    {
        public ConversionForm()
        {
            InitializeComponent();
        }

        public ConversionForm(string inputFile)
        {
            InitializeComponent();
            ConvertControl.SetSrcFile(inputFile);
        }

        private void AcceptButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
