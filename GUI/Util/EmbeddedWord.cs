using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DSOFramer;
using EmbeddedWord;
using System.Runtime.InteropServices;

namespace GUI.Util
{
    /// <summary>
    /// A user control in which Word is embedded. The docking mode of the embedded Word application is
    /// fill such that the its size is the same as the size of the whole control.
    /// A transparent panel lies over the embedded Word in order to prevent changes.
    /// </summary>
    public partial class EmbeddedWord : UserControl
    {
        /// <summary>
        /// The embedded word component. (Too bad I didn't found a way to do this via the designer.)
        /// </summary>
        public EmbeddedWordComponent Word;

        /// <summary>
        /// If an exception was thrown while initializing the component,
        /// it will be stored here, otherwise, it will be null.
        /// </summary>
        public COMException Exception;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmbeddedWord()
        {
            try
            {
                //ComponentResourceManager resources = new ComponentResourceManager(typeof(EmbeddedWord));

                Word = new EmbeddedWordComponent();

                ((ISupportInitialize) Word).BeginInit();

                InitializeComponent();

                Word.Enabled = true;
                Word.Location = new Point(0, 0);
                Word.Name = "EmbeddedWord";
                // Word.OcxState = ((AxHost.State)(resources.GetObject("EmbeddedWord.OcxState")));
                Word.TabIndex = 43;
                Word.Dock = DockStyle.Fill;
                Word.Margin = new Padding(0);
                Word.Margin = new Padding(0);
                Controls.Add(Word);
                ((ISupportInitialize) Word).EndInit();

                //Word.BorderStyle = dsoBorderStyle.dsoBorderFlat;
                Word.BorderStyle = dsoBorderStyle.dsoBorderNone;
                Word.Toolbars = false;
                Word.Titlebar = false;
            }
            catch (COMException exc)
            {
                Exception = exc;
            }
            catch (IOException ioe)
            {
                MessageBox.Show(@"Couldn't initialize the 'EmbeddedWord' module: " + ioe, @"System.IO.FileNotFoundException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}