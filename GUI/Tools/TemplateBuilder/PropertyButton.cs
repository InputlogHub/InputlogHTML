using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI.TemplateBuilder
{
    public partial class PropertyButton : UserControl
    {
        #region Fields

        public string Property { get; private set; }

        string ToolTip;
        #endregion

        public PropertyButton(string name, string tooltip)
        {
            ToolTip = tooltip;
            InitializeComponent();

            this.Property = name;
            this.btn_property.Text = Property;
            this.PropertyTooltip.SetToolTip(this.btn_property, ToolTip);
        }

        private void btn_property_Click(object sender, EventArgs e)
        {
            var last_block = (this.ParentForm as TemplateBuilder).LastBlock();

            if (last_block != null)
                last_block.AddPropertyToElements("[[" + this.Property + "]]");
        }
    }
}
