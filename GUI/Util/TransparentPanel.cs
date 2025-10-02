using System.Windows.Forms;

namespace GUI.Util
{
    /// <summary>
    /// A simple transparent panel.
    /// </summary>
    public partial class TransparentPanel : UserControl
    {
        /// <summary>
        /// I honestly do not know what this is, but it has to stay here.
        /// JR: who wrote this comment (RB I guess :P)?
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return createParams;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TransparentPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Override method so background is not drawn.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaintBackground(PaintEventArgs e) { }
    }
}